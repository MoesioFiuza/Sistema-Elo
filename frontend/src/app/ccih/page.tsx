"use client";

import { useCallback, useEffect, useState } from "react";
import { AuthGate } from "@/components/auth/AuthGate";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { StatCard } from "@/components/ui/StatCard";
import { api, formatarData } from "@/lib/api/client";
import type { DashboardResumo, Notificacao } from "@/lib/api/types";

export default function CcihPage() {
  return (
    <AuthGate perfis={["CCIH", "Enfermagem"]}>
      <CcihContent />
    </AuthGate>
  );
}

function CcihContent() {
  const [resumo, setResumo] = useState<DashboardResumo | null>(null);
  const [notifs, setNotifs] = useState<Notificacao[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const carregar = useCallback(async (silencioso = false) => {
    if (!silencioso) setLoading(true);
    setErro(null);
    try {
      const [dash, alerts] = await Promise.all([
        api.dashboard.resumo(),
        api.notificacoes.listar(true),
      ]);
      setResumo(dash);
      setNotifs(alerts);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao carregar painel");
      if (!silencioso) setResumo(null);
    } finally {
      if (!silencioso) setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregar();
    const interval = setInterval(() => carregar(true), 30_000);
    return () => clearInterval(interval);
  }, [carregar]);

  const maxEnfermaria = Math.max(
    ...(resumo?.porEnfermaria.map((e) => e.total) ?? [1]),
    1,
  );

  const isolamentoAtivo = resumo?.alertasRecentes.filter((a) => a.isolamentoAtivo) ?? [];

  return (
    <PageLayout
      badge="CCIH"
      title="Alerta e vigilância"
      subtitle="Atualização a cada 30 segundos"
      action={
        <button
          onClick={() => carregar()}
          className="rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Atualizar
        </button>
      }
    >
      {erro && <Alert variant="error">{erro}</Alert>}

      {isolamentoAtivo.length > 0 && (
        <div className="mb-6 rounded-xl border-2 border-red-500 bg-red-600 px-6 py-5 text-white shadow-lg">
          <p className="text-xs font-bold uppercase tracking-widest text-red-100">
            Protocolo de isolamento
          </p>
          <p className="mt-1 text-2xl font-extrabold tracking-tight sm:text-3xl">
            ISOLAR PACIENTE
          </p>
          <ul className="mt-3 space-y-1 text-sm text-red-50">
            {isolamentoAtivo.slice(0, 5).map((a) => (
              <li key={a.solicitacaoId}>
                {a.pacienteNome} · {a.enfermaria}
                {a.leito ? ` · Leito ${a.leito}` : ""} · {a.idAmostraUnico}
              </li>
            ))}
          </ul>
        </div>
      )}

      {loading && !resumo ? (
        <Spinner label="Carregando painel..." />
      ) : resumo ? (
        <>
          <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
            <StatCard label="Pendentes" value={resumo.solicitacoesPendentes} tone="warning" />
            <StatCard label="Em análise" value={resumo.emAnalise} />
            <StatCard label="Positivos" value={resumo.resultadosPositivos} tone="danger" />
            <StatCard label="Negativos" value={resumo.resultadosNegativos} tone="success" />
            <StatCard label="Isolamento" value={resumo.pacientesComIsolamento} tone="danger" />
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
              <h2 className="mb-5 font-semibold text-slate-900">Mapa por enfermaria</h2>
              {resumo.porEnfermaria.length === 0 ? (
                <p className="text-sm text-slate-500">Sem dados</p>
              ) : (
                <ul className="space-y-4">
                  {resumo.porEnfermaria.map((e) => (
                    <li key={e.enfermaria}>
                      <div className="mb-1 flex justify-between text-sm">
                        <span className="font-medium">{e.enfermaria}</span>
                        <span className="text-slate-500">
                          {e.total} exames
                          {e.positivos > 0 && (
                            <span className="ml-2 font-medium text-red-600">
                              {e.positivos} pos.
                            </span>
                          )}
                        </span>
                      </div>
                      <div className="h-2.5 overflow-hidden rounded-full bg-slate-100">
                        <div
                          className={`h-full rounded-full transition-all ${
                            e.positivos > 0 ? "bg-red-500" : "bg-teal-600"
                          }`}
                          style={{
                            width: `${Math.max(8, (e.total / maxEnfermaria) * 100)}%`,
                          }}
                        />
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>

            <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
              <h2 className="mb-5 font-semibold text-slate-900">Notificações</h2>
              {notifs.length === 0 && resumo.alertasRecentes.length === 0 ? (
                <p className="text-sm text-slate-500">Nenhum alerta</p>
              ) : (
                <ul className="space-y-3">
                  {notifs.map((n) => (
                    <li
                      key={n.id}
                      className={`rounded-lg border p-4 ${
                        n.tipo === "Isolamento"
                          ? "border-red-200 bg-red-50"
                          : "border-emerald-200 bg-emerald-50"
                      }`}
                    >
                      <p className="text-sm font-semibold text-slate-900">{n.titulo}</p>
                      <p className="mt-1 text-xs text-slate-600">{n.mensagem}</p>
                      <p className="mt-1 text-xs text-slate-400">{formatarData(n.criadoEm)}</p>
                    </li>
                  ))}
                  {notifs.length === 0 &&
                    resumo.alertasRecentes.map((a) => (
                      <li
                        key={a.solicitacaoId}
                        className="rounded-lg border border-red-200 bg-red-50 p-4"
                      >
                        <p className="text-sm font-semibold text-red-900">
                          Isolamento — {a.pacienteNome}
                        </p>
                        <p className="mt-1 text-xs text-red-700">
                          {a.enfermaria} · {a.idAmostraUnico} ·{" "}
                          {formatarData(a.dataResultado)}
                        </p>
                      </li>
                    ))}
                </ul>
              )}
            </div>
          </div>
        </>
      ) : null}
    </PageLayout>
  );
}
