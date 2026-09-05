"use client";

import { useCallback, useEffect, useState } from "react";
import { AuthGate } from "@/components/auth/AuthGate";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { api, formatarData } from "@/lib/api/client";
import type { SolicitacaoAcesso } from "@/lib/api/types";

export default function AdminPage() {
  return (
    <AuthGate perfis={["Admin"]}>
      <AdminContent />
    </AuthGate>
  );
}

function AdminContent() {
  const [pedidos, setPedidos] = useState<SolicitacaoAcesso[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [msg, setMsg] = useState<string | null>(null);

  const carregar = useCallback(async () => {
    setLoading(true);
    setErro(null);
    try {
      setPedidos(await api.acessos.listar());
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao carregar pedidos");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    carregar();
  }, [carregar]);

  async function aprovar(id: string) {
    setErro(null);
    setMsg(null);
    try {
      const res = await api.acessos.aprovar(id);
      setMsg(`Acesso criado para ${res.email}. Senha inicial: ${res.senhaInicial}`);
      await carregar();
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao aprovar");
    }
  }

  async function recusar(id: string) {
    const motivo = window.prompt("Motivo da recusa");
    if (!motivo?.trim()) return;
    setErro(null);
    setMsg(null);
    try {
      await api.acessos.recusar(id, motivo.trim());
      setMsg("Pedido recusado.");
      await carregar();
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao recusar");
    }
  }

  return (
    <PageLayout
      badge="Administração"
      title="Pedidos de acesso"
      subtitle="Apenas a equipe administrativa libera contas individuais"
    >
      {msg && <Alert variant="success">{msg}</Alert>}
      {erro && <Alert variant="error">{erro}</Alert>}

      <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        {loading ? (
          <Spinner />
        ) : pedidos.length === 0 ? (
          <p className="text-sm text-slate-500">Nenhum pedido ainda.</p>
        ) : (
          <ul className="divide-y divide-slate-100">
            {pedidos.map((p) => (
              <li key={p.id} className="flex flex-wrap items-start justify-between gap-4 py-4">
                <div>
                  <p className="font-semibold text-slate-900">{p.nome}</p>
                  <p className="text-sm text-slate-600">{p.email}</p>
                  <p className="mt-1 text-xs text-slate-500">
                    {p.perfilSolicitado}
                    {p.setor ? ` · ${p.setor}` : ""} · {formatarData(p.criadoEm)}
                  </p>
                  {p.justificativa && (
                    <p className="mt-1 text-sm text-slate-600">{p.justificativa}</p>
                  )}
                  {p.motivoRecusa && (
                    <p className="mt-1 text-sm text-red-700">Recusa: {p.motivoRecusa}</p>
                  )}
                </div>
                <div className="flex items-center gap-2">
                  <span
                    className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                      p.status === "Pendente"
                        ? "bg-amber-100 text-amber-800"
                        : p.status === "Aprovada"
                          ? "bg-emerald-100 text-emerald-800"
                          : "bg-red-100 text-red-800"
                    }`}
                  >
                    {p.status}
                  </span>
                  {p.status === "Pendente" && (
                    <>
                      <button
                        type="button"
                        onClick={() => aprovar(p.id)}
                        className="rounded-md bg-teal-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-teal-700"
                      >
                        Aprovar
                      </button>
                      <button
                        type="button"
                        onClick={() => recusar(p.id)}
                        className="rounded-md border border-red-200 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50"
                      >
                        Recusar
                      </button>
                    </>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </PageLayout>
  );
}
