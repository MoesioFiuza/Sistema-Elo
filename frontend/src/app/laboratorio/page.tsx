"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { AuthGate } from "@/components/auth/AuthGate";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { api, formatarData, statusLabel } from "@/lib/api/client";
import type { ResultadoTeste, Solicitacao } from "@/lib/api/types";

export default function LaboratorioPage() {
  return (
    <AuthGate perfis={["Laboratorio"]}>
      <LabContent />
    </AuthGate>
  );
}

function LabContent() {
  const [fila, setFila] = useState<Solicitacao[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [msg, setMsg] = useState<string | null>(null);
  const [selecionada, setSelecionada] = useState<Solicitacao | null>(null);
  const [resultado, setResultado] = useState({
    testeRapido: "Negativo" as ResultadoTeste,
    toxinaA: "NaoRegistrado" as ResultadoTeste,
    toxinaB: "NaoRegistrado" as ResultadoTeste,
    cepaIdentificada: "",
  });

  const carregar = useCallback(async (silencioso = false) => {
    if (!silencioso) setLoading(true);
    setErro(null);
    try {
      setFila(await api.solicitacoes.fila());
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

  async function receber(id: string) {
    setMsg(null);
    setErro(null);
    setFila((prev) =>
      prev.map((s) => (s.id === id ? { ...s, status: "EmAnalise" as const } : s)),
    );
    try {
      await api.solicitacoes.receber(id);
      setMsg("Recebimento confirmado.");
      await carregar(true);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao confirmar recebimento");
      await carregar(true);
    }
  }

  async function lancarResultado(e: FormEvent) {
    e.preventDefault();
    if (!selecionada) return;
    setMsg(null);
    setErro(null);
    try {
      const res = await api.solicitacoes.resultado(selecionada.id, {
        testeRapido: resultado.testeRapido,
        toxinaA: resultado.toxinaA,
        toxinaB: resultado.toxinaB,
        cepaIdentificada: resultado.cepaIdentificada || undefined,
      });
      setMsg(
        `Resultado salvo: ${res.idAmostraUnico}${
          res.resultado?.alertaPositivoEnviado ? " — alerta ISOLAR enviado" : " — liberação notificada"
        }`,
      );
      setSelecionada(null);
      setFila((prev) => prev.filter((s) => s.id !== res.id));
      await carregar(true);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao registrar resultado");
    }
  }

  return (
    <PageLayout
      badge="Laboratório"
      title="Fila de exames"
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

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm lg:col-span-3">
          <h2 className="mb-4 font-semibold text-slate-900">Solicitações ativas</h2>
          {loading ? (
            <Spinner label="Carregando fila..." />
          ) : !erro && fila.length === 0 ? (
            <p className="py-8 text-center text-sm text-slate-500">Fila vazia</p>
          ) : !erro ? (
            <ul className="space-y-3">
              {fila.map((s) => (
                <li
                  key={s.id}
                  className={`rounded-lg border p-4 ${
                    selecionada?.id === s.id
                      ? "border-teal-400 bg-teal-50/50"
                      : "border-slate-200"
                  }`}
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <p className="font-mono text-sm font-semibold text-slate-900">
                        {s.idAmostraUnico}
                      </p>
                      <p className="mt-1 font-medium">{s.pacienteNome}</p>
                      <p className="text-xs text-slate-500">
                        {s.enfermaria}
                        {s.leito ? ` · Leito ${s.leito}` : ""} ·{" "}
                        {formatarData(s.carimboDataHora)}
                      </p>
                    </div>
                    <span
                      className={`shrink-0 rounded-full px-2.5 py-0.5 text-xs font-medium ${
                        s.status === "Pendente"
                          ? "bg-amber-100 text-amber-800"
                          : "bg-blue-100 text-blue-800"
                      }`}
                    >
                      {statusLabel[s.status]}
                    </span>
                  </div>
                  <div className="mt-3 flex gap-2">
                    {s.status === "Pendente" && (
                      <button
                        onClick={() => receber(s.id)}
                        className="rounded-md bg-teal-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-teal-700"
                      >
                        Confirmar recebimento
                      </button>
                    )}
                    {s.status === "EmAnalise" && (
                      <button
                        onClick={() => setSelecionada(s)}
                        className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium hover:bg-slate-50"
                      >
                        Lançar resultado
                      </button>
                    )}
                  </div>
                </li>
              ))}
            </ul>
          ) : null}
        </div>

        <form
          onSubmit={lancarResultado}
          className="h-fit rounded-xl border border-slate-200 bg-white p-6 shadow-sm lg:col-span-2"
        >
          <h2 className="mb-4 font-semibold text-slate-900">Resultado</h2>
          {selecionada ? (
            <div className="space-y-3">
              <p className="text-sm text-slate-500">
                Amostra{" "}
                <strong className="font-mono text-slate-900">
                  {selecionada.idAmostraUnico}
                </strong>
              </p>
              <ResultSelect
                label="Teste rápido"
                value={resultado.testeRapido}
                onChange={(v) => setResultado({ ...resultado, testeRapido: v })}
                required
              />
              <ResultSelect
                label="Toxina A"
                value={resultado.toxinaA}
                onChange={(v) => setResultado({ ...resultado, toxinaA: v })}
              />
              <ResultSelect
                label="Toxina B"
                value={resultado.toxinaB}
                onChange={(v) => setResultado({ ...resultado, toxinaB: v })}
              />
              <div>
                <label className="mb-1.5 block text-sm font-medium text-slate-700">Cepa</label>
                <input
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  placeholder="Ex.: RT027"
                  value={resultado.cepaIdentificada}
                  onChange={(e) =>
                    setResultado({ ...resultado, cepaIdentificada: e.target.value })
                  }
                />
              </div>
              <button
                type="submit"
                className="w-full rounded-lg bg-teal-600 py-2.5 text-sm font-medium text-white hover:bg-teal-700"
              >
                Salvar
              </button>
            </div>
          ) : (
            <p className="text-sm text-slate-500">
              Selecione uma amostra em análise.
            </p>
          )}
        </form>
      </div>
    </PageLayout>
  );
}

function ResultSelect({
  label,
  value,
  onChange,
  required,
}: {
  label: string;
  value: ResultadoTeste;
  onChange: (v: ResultadoTeste) => void;
  required?: boolean;
}) {
  return (
    <div>
      <label className="mb-1.5 block text-sm font-medium text-slate-700">{label}</label>
      <select
        required={required}
        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
        value={value}
        onChange={(e) => onChange(e.target.value as ResultadoTeste)}
      >
        {!required && <option value="NaoRegistrado">Não registrado</option>}
        <option value="Positivo">Positivo</option>
        <option value="Negativo">Negativo</option>
        <option value="Indeterminado">Indeterminado</option>
      </select>
    </div>
  );
}
