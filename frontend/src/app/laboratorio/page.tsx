"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { AuthGate } from "@/components/auth/AuthGate";
import { SignaturePad } from "@/components/lab/SignaturePad";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { api, formatarData, resultadoLabel, statusLabel } from "@/lib/api/client";
import type { ResultadoTeste, Solicitacao, SolicitacaoDetalhe } from "@/lib/api/types";

const TRILHA = [
  { id: "Pendente", label: "Solicitação em andamento" },
  { id: "Coletado", label: "Coleta realizada" },
  { id: "EmAnalise", label: "Testagem em andamento" },
  { id: "ResultadoLiberado", label: "Testagem realizada" },
] as const;

export default function LaboratorioPage() {
  return (
    <AuthGate perfis={["Laboratorio"]}>
      <LabContent />
    </AuthGate>
  );
}

function LabContent() {
  const [fila, setFila] = useState<Solicitacao[]>([]);
  const [historico, setHistorico] = useState<Solicitacao[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [msg, setMsg] = useState<string | null>(null);
  const [selecionada, setSelecionada] = useState<Solicitacao | null>(null);
  const [detalhe, setDetalhe] = useState<SolicitacaoDetalhe | null>(null);
  const [aba, setAba] = useState<"fila" | "historico">("fila");
  const [resultado, setResultado] = useState({
    testeRapido: "Negativo" as ResultadoTeste,
    cultura: "Negativo" as ResultadoTeste,
    cepaIdentificada: "",
    observacoesLaboratorio: "",
    assinadoPorNome: "",
    assinaturaBase64: "" as string,
  });
  const [anexo, setAnexo] = useState<File | null>(null);

  const carregar = useCallback(async (silencioso = false) => {
    if (!silencioso) setLoading(true);
    setErro(null);
    try {
      const [f, h] = await Promise.all([
        api.solicitacoes.fila(),
        api.solicitacoes.historico(),
      ]);
      setFila(f);
      setHistorico(h);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao carregar fila");
      setFila([]);
    } finally {
      if (!silencioso) setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregar();
  }, [carregar]);

  async function selecionar(s: Solicitacao) {
    setSelecionada(s);
    try {
      setDetalhe(await api.solicitacoes.obter(s.id));
    } catch {
      setDetalhe(null);
    }
  }

  async function registrarColeta(id: string) {
    setMsg(null);
    setErro(null);
    try {
      const res = await api.solicitacoes.coleta(id);
      setMsg("Coleta registrada. Avalie se a amostra é satisfatória.");
      setSelecionada({ ...res, testeRapido: res.testeRapido, cultura: res.cultura });
      setDetalhe(res);
      await carregar(true);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao registrar coleta");
    }
  }

  async function avaliar(id: string, qualidade: "Satisfatoria" | "Insatisfatoria") {
    setMsg(null);
    setErro(null);
    try {
      const res = await api.solicitacoes.amostra(id, qualidade);
      setMsg(
        qualidade === "Satisfatoria"
          ? "Amostra satisfatória. Pode lançar o resultado."
          : "Amostra insatisfatória. Solicite nova coleta se necessário.",
      );
      setSelecionada({ ...res, testeRapido: res.testeRapido, cultura: res.cultura });
      setDetalhe(res);
      await carregar(true);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao avaliar amostra");
    }
  }

  async function lancarResultado(e: FormEvent) {
    e.preventDefault();
    if (!selecionada) return;
    if (!resultado.assinaturaBase64) {
      setErro("Desenhe a assinatura do responsável. Ela é obrigatória para liberar o laudo.");
      return;
    }
    if (!resultado.assinadoPorNome.trim()) {
      setErro("Informe o nome de quem assina o laudo.");
      return;
    }
    setMsg(null);
    setErro(null);
    try {
      const res = await api.solicitacoes.resultado(selecionada.id, {
        testeRapido: resultado.testeRapido,
        cultura: resultado.cultura,
        cepaIdentificada: resultado.cepaIdentificada || undefined,
        observacoesLaboratorio: resultado.observacoesLaboratorio || undefined,
        assinaturaBase64: resultado.assinaturaBase64 || undefined,
        assinadoPorNome: resultado.assinadoPorNome || undefined,
      });
      if (anexo) {
        await api.solicitacoes.anexarLaudo(res.id, anexo);
      }
      setMsg(`Testagem realizada: ${res.idAmostraUnico}. O laudo pode ser impresso.`);
      setSelecionada(null);
      setDetalhe(null);
      setAnexo(null);
      await carregar(true);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao registrar resultado");
    }
  }

  const lista = aba === "fila" ? fila : historico;

  return (
    <PageLayout
      badge="Laboratório"
      title="Fila e laudos"
      subtitle="Trilha: solicitação em andamento → coleta realizada → testagem realizada"
      action={
        <button
          onClick={() => carregar()}
          className="rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Atualizar
        </button>
      }
    >
      {msg && <Alert variant="success">{msg}</Alert>}
      {erro && <Alert variant="error">{erro}</Alert>}

      <div className="mb-4 flex gap-2">
        <button
          type="button"
          onClick={() => setAba("fila")}
          className={`rounded-lg px-4 py-2 text-sm font-medium ${
            aba === "fila" ? "bg-teal-600 text-white" : "border border-slate-200 bg-white"
          }`}
        >
          Fila ativa
        </button>
        <button
          type="button"
          onClick={() => setAba("historico")}
          className={`rounded-lg px-4 py-2 text-sm font-medium ${
            aba === "historico" ? "bg-teal-600 text-white" : "border border-slate-200 bg-white"
          }`}
        >
          Coletas anteriores
        </button>
      </div>

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm lg:col-span-3">
          <h2 className="mb-4 font-semibold text-slate-900">
            {aba === "fila" ? "Amostras em andamento" : "Resultados já liberados"}
          </h2>
          {loading ? (
            <Spinner label="Carregando..." />
          ) : lista.length === 0 ? (
            <p className="py-8 text-center text-sm text-slate-500">Nenhuma amostra nesta lista</p>
          ) : (
            <ul className="space-y-3">
              {lista.map((s) => (
                <li
                  key={s.id}
                  className={`rounded-lg border p-4 ${
                    selecionada?.id === s.id ? "border-teal-400 bg-teal-50/50" : "border-slate-200"
                  }`}
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <p className="font-mono text-sm font-semibold text-slate-900">
                        {s.idAmostraUnico}
                      </p>
                      <p className="mt-1 font-medium">{s.pacienteNome}</p>
                      <p className="text-xs text-slate-500">
                        Prontuário {s.numeroProntuario} · {s.enfermaria}
                        {s.leito ? ` · Leito ${s.leito}` : ""} · {formatarData(s.carimboDataHora)}
                      </p>
                    </div>
                    <span className={`shrink-0 rounded-full px-2.5 py-0.5 text-xs font-medium ${badgeClass(s.status)}`}>
                      {statusLabel[s.status] ?? s.status}
                    </span>
                  </div>
                  <StatusTrail status={s.status} />
                  <div className="mt-3 flex flex-wrap gap-2">
                    {s.status === "Pendente" && aba === "fila" && (
                      <button
                        onClick={() => registrarColeta(s.id)}
                        className="rounded-md bg-teal-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-teal-700"
                      >
                        Registrar coleta
                      </button>
                    )}
                    {s.status === "AmostraInsatisfatoria" && aba === "fila" && (
                      <button
                        onClick={() => registrarColeta(s.id)}
                        className="rounded-md bg-amber-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-amber-700"
                      >
                        Nova coleta
                      </button>
                    )}
                    {s.status === "Coletado" && aba === "fila" && (
                      <>
                        <button
                          onClick={() => avaliar(s.id, "Satisfatoria")}
                          className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-emerald-700"
                        >
                          Amostra satisfatória
                        </button>
                        <button
                          onClick={() => avaliar(s.id, "Insatisfatoria")}
                          className="rounded-md border border-red-200 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50"
                        >
                          Amostra insatisfatória
                        </button>
                      </>
                    )}
                    {(s.status === "EmAnalise" || s.status === "ResultadoLiberado") && (
                      <button
                        onClick={() => selecionar(s)}
                        className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium hover:bg-slate-50"
                      >
                        {s.status === "EmAnalise" ? "Lançar resultado" : "Ver laudo"}
                      </button>
                    )}
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="h-fit rounded-xl border border-slate-200 bg-white p-6 shadow-sm lg:col-span-2">
          {!selecionada ? (
            <p className="text-sm text-slate-500">
              Selecione uma amostra em testagem ou um resultado já liberado.
            </p>
          ) : selecionada.status === "ResultadoLiberado" ? (
            <div className="space-y-3">
              <h2 className="font-semibold text-slate-900">Laudo</h2>
              <p className="font-mono text-sm">{selecionada.idAmostraUnico}</p>
              {detalhe?.resultado && (
                <div className="space-y-2 text-sm">
                  <p>
                    <strong>Teste rápido:</strong>{" "}
                    {resultadoLabel[detalhe.resultado.testeRapido]}
                  </p>
                  <p>
                    <strong>Cultura:</strong> {resultadoLabel[detalhe.resultado.cultura]}
                  </p>
                  {detalhe.resultado.assinadoPorNome && (
                    <p className="text-slate-500">
                      Assinado por {detalhe.resultado.assinadoPorNome}
                    </p>
                  )}
                  {detalhe.resultado.laudoAnexoNome && (
                    <p className="text-slate-500">Anexo: {detalhe.resultado.laudoAnexoNome}</p>
                  )}
                </div>
              )}
              <a
                href={`/laboratorio/laudo/${selecionada.id}`}
                className="inline-flex w-full justify-center rounded-lg bg-teal-600 px-4 py-2 text-sm font-medium text-white hover:bg-teal-700"
              >
                Abrir / imprimir laudo
              </a>
              {detalhe?.resultado?.laudoAnexoNome && (
                <button
                  type="button"
                  onClick={() =>
                    api.solicitacoes.baixarAnexo(selecionada.id).catch((e) =>
                      setErro(e instanceof Error ? e.message : "Falha ao baixar anexo"),
                    )
                  }
                  className="inline-flex w-full justify-center rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium hover:bg-slate-50"
                >
                  Baixar laudo anexado
                </button>
              )}
            </div>
          ) : (
            <form onSubmit={lancarResultado} className="space-y-4">
              <h2 className="font-semibold text-slate-900">Resultado</h2>
              <p className="text-sm text-slate-500">
                Amostra{" "}
                <strong className="font-mono text-slate-900">{selecionada.idAmostraUnico}</strong>
              </p>
              <ResultBox
                title="RESULTADO DO TESTE RÁPIDO"
                value={resultado.testeRapido}
                onChange={(v) => setResultado({ ...resultado, testeRapido: v })}
              />
              <ResultBox
                title="RESULTADO DA CULTURA"
                value={resultado.cultura}
                onChange={(v) => setResultado({ ...resultado, cultura: v })}
              />
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">
                  Cepa (opcional)
                </label>
                <input
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  placeholder="Ex.: RT027"
                  value={resultado.cepaIdentificada}
                  onChange={(e) =>
                    setResultado({ ...resultado, cepaIdentificada: e.target.value })
                  }
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">
                  Nome de quem assina *
                </label>
                <input
                  required
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={resultado.assinadoPorNome}
                  onChange={(e) =>
                    setResultado({ ...resultado, assinadoPorNome: e.target.value })
                  }
                />
              </div>
              <SignaturePad
                onChange={(data) =>
                  setResultado({ ...resultado, assinaturaBase64: data ?? "" })
                }
              />
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">
                  Anexar laudo (PDF, PNG ou JPG)
                </label>
                <input
                  type="file"
                  accept="application/pdf,image/png,image/jpeg"
                  onChange={(e) => setAnexo(e.target.files?.[0] ?? null)}
                  className="w-full text-sm"
                />
              </div>
              <button
                type="submit"
                className="w-full rounded-lg bg-teal-600 py-2.5 text-sm font-medium text-white hover:bg-teal-700"
              >
                Salvar resultado e gerar laudo
              </button>
            </form>
          )}
        </div>
      </div>
    </PageLayout>
  );
}

function StatusTrail({ status }: { status: string }) {
  const atual =
    status === "AmostraInsatisfatoria"
      ? 1
      : TRILHA.findIndex((t) => t.id === status);
  return (
    <ol className="mt-3 flex flex-wrap gap-1.5">
      {TRILHA.map((passo, i) => {
        const done = atual >= i;
        return (
          <li
            key={passo.id}
            className={`rounded-full px-2 py-0.5 text-[10px] font-medium ${
              done ? "bg-teal-100 text-teal-800" : "bg-slate-100 text-slate-400"
            }`}
          >
            {i + 1}. {passo.label}
          </li>
        );
      })}
    </ol>
  );
}

function badgeClass(status: string) {
  switch (status) {
    case "Pendente":
      return "bg-amber-100 text-amber-800";
    case "Coletado":
      return "bg-sky-100 text-sky-800";
    case "EmAnalise":
      return "bg-blue-100 text-blue-800";
    case "ResultadoLiberado":
      return "bg-emerald-100 text-emerald-800";
    case "AmostraInsatisfatoria":
      return "bg-red-100 text-red-800";
    default:
      return "bg-slate-100 text-slate-700";
  }
}

function ResultBox({
  title,
  value,
  onChange,
}: {
  title: string;
  value: ResultadoTeste;
  onChange: (v: ResultadoTeste) => void;
}) {
  return (
    <fieldset className="rounded-xl border-2 border-slate-200 p-3">
      <legend className="px-1 text-xs font-bold tracking-wide text-slate-700">{title}</legend>
      <div className="flex flex-wrap gap-2">
        {(["Positivo", "Negativo", "Indeterminado"] as ResultadoTeste[]).map((op) => (
          <label
            key={op}
            className={`cursor-pointer rounded-lg border px-3 py-1.5 text-sm ${
              value === op
                ? op === "Positivo"
                  ? "border-red-400 bg-red-50 font-semibold text-red-800"
                  : "border-teal-400 bg-teal-50 font-semibold text-teal-800"
                : "border-slate-200 bg-white"
            }`}
          >
            <input
              type="radio"
              className="sr-only"
              checked={value === op}
              onChange={() => onChange(op)}
            />
            {op}
          </label>
        ))}
      </div>
    </fieldset>
  );
}
