import { authHeader, clearAuth } from "@/lib/auth";
import type {
  CepaDesfecho,
  DashboardResumo,
  FormularioClinicoPayload,
  InternacaoFichaPayload,
  Laudo,
  LoginResponse,
  Notificacao,
  Paciente,
  PacienteDetalhe,
  PacienteHistoricoPayload,
  QualidadeAmostra,
  ResultadoTeste,
  SimNao,
  Solicitacao,
  SolicitacaoAcesso,
  SolicitacaoDetalhe,
  Sexo,
  Tratamento,
  RespostaClinica,
} from "./types";

const API_BASE_URL = "/api";
const TIMEOUT_MS = 8_000;

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), TIMEOUT_MS);

  try {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      signal: controller.signal,
      headers: {
        "Content-Type": "application/json",
        ...authHeader(),
        ...options?.headers,
      },
      cache: "no-store",
    });

    if (response.status === 401) {
      clearAuth();
      if (typeof window !== "undefined" && !window.location.pathname.startsWith("/login")) {
        window.location.href = "/login";
      }
      throw new ApiError(401, "Sessão expirada. Faça login novamente.");
    }

    if (!response.ok) {
      let message = response.statusText;
      try {
        const body = await response.json();
        message = body.erro ?? body.title ?? message;
      } catch {
        message = await response.text().catch(() => message);
      }
      throw new ApiError(response.status, message || "Erro na requisição");
    }

    if (response.status === 204) return undefined as T;
    return response.json() as Promise<T>;
  } catch (error) {
    if (error instanceof ApiError) throw error;
    if (error instanceof DOMException && error.name === "AbortError") {
      throw new ApiError(
        408,
        "API não respondeu a tempo. Confira se o backend está rodando (porta 5000).",
      );
    }
    throw new ApiError(
      0,
      "Sem conexão com a API. Rode: docker compose up postgres -d && dotnet run --project Elo.Api",
    );
  } finally {
    clearTimeout(timer);
  }
}

export const api = {
  health: () =>
    request<{ status: string; sistema: string; versao: string }>("/health"),

  auth: {
    login: (email: string, senha: string) =>
      request<LoginResponse>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, senha }),
      }),
    solicitarAcesso: (data: {
      nome: string;
      email: string;
      perfilSolicitado: string;
      setor?: string;
      justificativa?: string;
    }) =>
      request<{ mensagem: string; id: string }>("/auth/solicitar-acesso", {
        method: "POST",
        body: JSON.stringify(data),
      }),
  },

  acessos: {
    listar: () => request<SolicitacaoAcesso[]>("/acessos"),
    aprovar: (id: string, senhaInicial?: string) =>
      request<{
        usuarioId: string;
        email: string;
        nome: string;
        perfil: string;
        senhaInicial: string;
      }>(`/acessos/${id}/aprovar`, {
        method: "POST",
        body: JSON.stringify({ senhaInicial: senhaInicial || undefined }),
      }),
    recusar: (id: string, motivo: string) =>
      request<void>(`/acessos/${id}/recusar`, {
        method: "POST",
        body: JSON.stringify({ motivo }),
      }),
  },

  pacientes: {
    buscar: (q?: string) =>
      request<Paciente[]>(`/pacientes${q ? `?q=${encodeURIComponent(q)}` : ""}`),
    obter: (id: string) => request<PacienteDetalhe>(`/pacientes/${id}`),
    criar: (data: {
      numeroProntuario: string;
      nome: string;
      dataNascimento?: string;
      sexo: Sexo;
      historicoDiarreiaPrevia?: SimNao;
      historicoCdiff?: SimNao;
      historicoCovid?: SimNao;
      enfermaria: string;
      leito?: string;
    }) =>
      request<PacienteDetalhe>("/pacientes", {
        method: "POST",
        body: JSON.stringify(data),
      }),
  },

  solicitacoes: {
    listar: (status?: string, pacienteId?: string) => {
      const params = new URLSearchParams();
      if (status) params.set("status", status);
      if (pacienteId) params.set("pacienteId", pacienteId);
      const qs = params.toString();
      return request<Solicitacao[]>(`/solicitacoes${qs ? `?${qs}` : ""}`);
    },
    fila: () => request<Solicitacao[]>("/solicitacoes/fila"),
    historico: () => request<Solicitacao[]>("/solicitacoes/historico"),
    obter: (id: string) => request<SolicitacaoDetalhe>(`/solicitacoes/${id}`),
    criar: (data: {
      pacienteId: string;
      internacaoId: string;
      formulario: FormularioClinicoPayload;
      internacao?: InternacaoFichaPayload;
      historicoPaciente?: PacienteHistoricoPayload;
    }) =>
      request<SolicitacaoDetalhe>("/solicitacoes", {
        method: "POST",
        body: JSON.stringify(data),
      }),
    coleta: (id: string) =>
      request<SolicitacaoDetalhe>(`/solicitacoes/${id}/coleta`, {
        method: "POST",
      }),
    amostra: (id: string, qualidade: QualidadeAmostra) =>
      request<SolicitacaoDetalhe>(`/solicitacoes/${id}/amostra`, {
        method: "POST",
        body: JSON.stringify({ qualidade }),
      }),
    resultado: (
      id: string,
      data: {
        testeRapido: ResultadoTeste;
        cultura: ResultadoTeste;
        cepaIdentificada?: string;
        observacoesLaboratorio?: string;
        assinaturaBase64?: string;
        assinadoPorNome?: string;
      },
    ) =>
      request<SolicitacaoDetalhe>(`/solicitacoes/${id}/resultado`, {
        method: "POST",
        body: JSON.stringify(data),
      }),
    laudo: (id: string) => request<Laudo>(`/solicitacoes/${id}/laudo`),
    baixarAnexo: async (id: string) => {
      const response = await fetch(`${API_BASE_URL}/solicitacoes/${id}/laudo-anexo`, {
        headers: { ...authHeader() },
        cache: "no-store",
      });
      if (!response.ok) {
        throw new ApiError(response.status, "Não foi possível baixar o laudo anexado.");
      }
      const blob = await response.blob();
      const disposition = response.headers.get("content-disposition") ?? "";
      const match = /filename\*?=(?:UTF-8'')?["']?([^"';]+)/i.exec(disposition);
      const nome = match ? decodeURIComponent(match[1]) : "laudo-anexo";
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = nome;
      a.click();
      URL.revokeObjectURL(url);
    },
    anexarLaudo: async (id: string, arquivo: File) => {
      const controller = new AbortController();
      const timer = setTimeout(() => controller.abort(), 20_000);
      try {
        const form = new FormData();
        form.append("arquivo", arquivo);
        const response = await fetch(`${API_BASE_URL}/solicitacoes/${id}/laudo-anexo`, {
          method: "POST",
          headers: { ...authHeader() },
          body: form,
          signal: controller.signal,
        });
        if (!response.ok) {
          let message = response.statusText;
          try {
            const body = await response.json();
            message = body.erro ?? message;
          } catch {
            /* ignore */
          }
          throw new ApiError(response.status, message || "Falha ao anexar laudo");
        }
        return response.json() as Promise<SolicitacaoDetalhe>;
      } finally {
        clearTimeout(timer);
      }
    },
  },

  dashboard: {
    resumo: () => request<DashboardResumo>("/dashboard/resumo"),
  },

  notificacoes: {
    listar: (naoLidas = false) =>
      request<Notificacao[]>(`/notificacoes?naoLidas=${naoLidas}`),
    marcarLida: (id: string) =>
      request<void>(`/notificacoes/${id}/lida`, { method: "POST" }),
  },

  pesquisa: {
    tratamentos: () => request<Tratamento[]>("/tratamentos"),
    salvarTratamento: (data: {
      solicitacaoExameId: string;
      iniciouTratamento?: SimNao;
      medicacao?: string;
      dose?: string;
      duracaoDias?: number;
      respostaDia7?: RespostaClinica;
      respostaFinal?: RespostaClinica;
      recidiva?: SimNao;
      observacoesTratamento?: string;
    }) =>
      request<Tratamento>("/tratamentos", {
        method: "POST",
        body: JSON.stringify(data),
      }),
    alta: (internacaoId: string, obito = false) =>
      request<void>(`/internacoes/${internacaoId}/alta`, {
        method: "POST",
        body: JSON.stringify({ obito }),
      }),
    cepaDesfecho: () => request<CepaDesfecho[]>("/pesquisa/cepa-desfecho"),
  },
};

export function formatarData(iso: string) {
  return new Date(iso).toLocaleString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export const statusLabel: Record<string, string> = {
  Pendente: "Solicitação em andamento",
  Coletado: "Coleta realizada",
  EmAnalise: "Testagem em andamento",
  ResultadoLiberado: "Testagem realizada",
  Cancelado: "Cancelado",
  AmostraInsatisfatoria: "Amostra insatisfatória",
};

export const resultadoLabel: Record<string, string> = {
  NaoRegistrado: "Não registrado",
  Positivo: "Positivo",
  Negativo: "Negativo",
  Indeterminado: "Indeterminado",
};
