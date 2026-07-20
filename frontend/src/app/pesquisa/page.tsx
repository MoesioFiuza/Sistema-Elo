"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { AuthGate } from "@/components/auth/AuthGate";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { api } from "@/lib/api/client";
import type { CepaDesfecho, RespostaClinica, SimNao, Solicitacao, Tratamento } from "@/lib/api/types";

export default function PesquisaPage() {
  return (
    <AuthGate perfis={["CCIH", "Medico", "Admin"]}>
      <PesquisaContent />
    </AuthGate>
  );
}

function PesquisaContent() {
  const [tratamentos, setTratamentos] = useState<Tratamento[]>([]);
  const [cepas, setCepas] = useState<CepaDesfecho[]>([]);
  const [liberadas, setLiberadas] = useState<Solicitacao[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [msg, setMsg] = useState<string | null>(null);

  const [form, setForm] = useState({
    solicitacaoExameId: "",
    medicacao: "Vancomicina",
    dose: "",
    duracaoDias: "10",
    respostaDia7: "NaoRegistrado" as RespostaClinica,
    respostaFinal: "NaoRegistrado" as RespostaClinica,
    recidiva: "NaoRegistrado" as SimNao,
  });

  const carregar = useCallback(async () => {
    setLoading(true);
    setErro(null);
    try {
      const [t, c, sols] = await Promise.all([
        api.pesquisa.tratamentos(),
        api.pesquisa.cepaDesfecho(),
        api.solicitacoes.listar("ResultadoLiberado"),
      ]);
      setTratamentos(t);
      setCepas(c);
      setLiberadas(sols);
      if (!form.solicitacaoExameId && sols[0]) {
        setForm((f) => ({ ...f, solicitacaoExameId: sols[0].id }));
      }
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao carregar pesquisa");
    } finally {
      setLoading(false);
    }
  }, [form.solicitacaoExameId]);

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function salvar(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setMsg(null);
    try {
      await api.pesquisa.salvarTratamento({
        solicitacaoExameId: form.solicitacaoExameId,
        iniciouTratamento: "Sim",
        medicacao: form.medicacao,
        dose: form.dose || undefined,
        duracaoDias: form.duracaoDias ? Number(form.duracaoDias) : undefined,
        respostaDia7: form.respostaDia7,
        respostaFinal: form.respostaFinal,
        recidiva: form.recidiva,
      });
      setMsg("Tratamento registrado.");
      await carregar();
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro ao salvar");
    }
  }

  return (
    <PageLayout
      badge="Pesquisa"
      title="Pesquisa e desfecho"
      subtitle="Antibioticoterapia, desfecho e análise cepa × resultado"
    >
      {msg && <Alert variant="success">{msg}</Alert>}
      {erro && <Alert variant="error">{erro}</Alert>}

      {loading ? (
        <Spinner />
      ) : (
        <div className="grid gap-6 lg:grid-cols-2">
          <form
            onSubmit={salvar}
            className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
          >
            <h2 className="font-semibold text-slate-900">Registrar tratamento</h2>
            <div>
              <label className="mb-1.5 block text-sm font-medium">Solicitação (resultado liberado)</label>
              <select
                required
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                value={form.solicitacaoExameId}
                onChange={(e) => setForm({ ...form, solicitacaoExameId: e.target.value })}
              >
                <option value="">Selecione</option>
                {liberadas.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.idAmostraUnico} — {s.pacienteNome}
                  </option>
                ))}
              </select>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <label className="mb-1.5 block text-sm font-medium">Medicação</label>
                <input
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.medicacao}
                  onChange={(e) => setForm({ ...form, medicacao: e.target.value })}
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium">Dose</label>
                <input
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.dose}
                  onChange={(e) => setForm({ ...form, dose: e.target.value })}
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium">Duração (dias)</label>
                <input
                  type="number"
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.duracaoDias}
                  onChange={(e) => setForm({ ...form, duracaoDias: e.target.value })}
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium">
                  Resposta clínica no 7º dia
                </label>
                <select
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.respostaDia7}
                  onChange={(e) =>
                    setForm({ ...form, respostaDia7: e.target.value as RespostaClinica })
                  }
                >
                  <option value="NaoRegistrado">Não registrado</option>
                  <option value="Cura">Resolução da diarreia</option>
                  <option value="Melhora">Melhora parcial</option>
                  <option value="SemMelhora">Sem melhora</option>
                  <option value="Piora">Piora da diarreia</option>
                </select>
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium">
                  Resposta clínica ao final do tratamento
                </label>
                <select
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.respostaFinal}
                  onChange={(e) =>
                    setForm({ ...form, respostaFinal: e.target.value as RespostaClinica })
                  }
                >
                  <option value="NaoRegistrado">Não registrado</option>
                  <option value="Cura">Resolução da diarreia</option>
                  <option value="Melhora">Melhora parcial</option>
                  <option value="SemMelhora">Sem melhora</option>
                  <option value="Piora">Piora da diarreia</option>
                </select>
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium">
                  Recidiva após o término?
                </label>
                <select
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                  value={form.recidiva}
                  onChange={(e) =>
                    setForm({ ...form, recidiva: e.target.value as SimNao })
                  }
                >
                  <option value="NaoRegistrado">Não registrado</option>
                  <option value="Sim">Sim</option>
                  <option value="Nao">Não</option>
                </select>
              </div>
            </div>
            <button
              type="submit"
              className="rounded-lg bg-teal-600 px-5 py-2 text-sm font-medium text-white hover:bg-teal-700"
            >
              Salvar tratamento
            </button>
          </form>

          <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <h2 className="mb-4 font-semibold text-slate-900">Cepa × desfecho</h2>
            {cepas.length === 0 ? (
              <p className="text-sm text-slate-500">
                Sem dados de cepa ainda. Lance resultados com cepa no laboratório.
              </p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead>
                    <tr className="border-b text-slate-500">
                      <th className="py-2 pr-3">Cepa</th>
                      <th className="py-2 pr-3">Total</th>
                      <th className="py-2 pr-3">Melhora</th>
                      <th className="py-2 pr-3">Recidiva</th>
                      <th className="py-2">Óbito</th>
                    </tr>
                  </thead>
                  <tbody>
                    {cepas.map((c) => (
                      <tr key={c.cepa} className="border-b border-slate-100">
                        <td className="py-2 pr-3 font-medium">{c.cepa}</td>
                        <td className="py-2 pr-3">{c.total}</td>
                        <td className="py-2 pr-3">{c.comMelhora}</td>
                        <td className="py-2 pr-3">{c.comRecidiva}</td>
                        <td className="py-2">{c.comObito}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            <h3 className="mb-3 mt-8 font-semibold text-slate-900">Tratamentos registrados</h3>
            {tratamentos.length === 0 ? (
              <p className="text-sm text-slate-500">Nenhum tratamento</p>
            ) : (
              <ul className="space-y-2 text-sm">
                {tratamentos.map((t) => (
                  <li key={t.id} className="rounded-lg border border-slate-100 px-3 py-2">
                    <span className="font-medium">{t.pacienteNome}</span>
                    <span className="text-slate-500">
                      {" "}
                      · {t.medicacao ?? "—"} · {t.idAmostraUnico}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      )}
    </PageLayout>
  );
}
