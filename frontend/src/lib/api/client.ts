import { authHeader, clearAuth } from "@/lib/auth";
import type {
  CepaDesfecho,
  DashboardResumo,
  FormularioClinicoPayload,
  InternacaoFichaPayload,
  LoginResponse,
  Notificacao,
  Paciente,
  PacienteDetalhe,
  PacienteHistoricoPayload,
  ResultadoTeste,
  SimNao,
  Solicitacao,
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
    listar: (status?: string) =>
      request<Solicitacao[]>(
        `/solicitacoes${status ? `?status=${status}` : ""}`,
      ),
    fila: () => request<Solicitacao[]>("/solicitacoes/fila"),
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
    receber: (id: string) =>
      request<SolicitacaoDetalhe>(`/solicitacoes/${id}/receber`, {
        method: "POST",
      }),
    resultado: (
      id: string,
      data: {
        testeRapido: ResultadoTeste;
        toxinaA?: ResultadoTeste;
        toxinaB?: ResultadoTeste;
        cultura?: ResultadoTeste;
        cepaIdentificada?: string;
      },
    ) =>
      request<SolicitacaoDetalhe>(`/solicitacoes/${id}/resultado`, {
        method: "POST",
        body: JSON.stringify(data),
      }),
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
  Pendente: "Pendente",
  Coletado: "Coletado",
  EmAnalise: "Em análise",
  ResultadoLiberado: "Resultado liberado",
  Cancelado: "Cancelado",
};
