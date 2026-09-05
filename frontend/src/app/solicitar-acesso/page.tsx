"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";
import { Header } from "@/components/layout/Header";
import { api } from "@/lib/api/client";

const PERFIS = [
  { value: "Medico", label: "Médico" },
  { value: "Laboratorio", label: "Laboratório" },
  { value: "CCIH", label: "CCIH" },
  { value: "Enfermagem", label: "Enfermagem" },
];

export default function SolicitarAcessoPage() {
  const [form, setForm] = useState({
    nome: "",
    email: "",
    perfilSolicitado: "Medico",
    setor: "",
    justificativa: "",
  });
  const [erro, setErro] = useState<string | null>(null);
  const [ok, setOk] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setOk(null);
    setLoading(true);
    try {
      const res = await api.auth.solicitarAcesso({
        nome: form.nome,
        email: form.email,
        perfilSolicitado: form.perfilSolicitado,
        setor: form.setor || undefined,
        justificativa: form.justificativa || undefined,
      });
      setOk(res.mensagem);
      setForm({ nome: "", email: "", perfilSolicitado: "Medico", setor: "", justificativa: "" });
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Não foi possível enviar o pedido");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <Header showLogin={false} backHref="/" />

      <div className="mx-auto max-w-md px-4 py-12 sm:px-6">
        <div className="rounded-xl border border-slate-200 bg-white p-8 shadow-sm">
          <p className="text-xs font-semibold uppercase tracking-[0.15em] text-teal-700">
            Cdigital · NEPEC
          </p>
          <h1 className="mt-1 text-xl font-bold text-slate-900">Solicitar acesso</h1>
          <p className="mt-2 text-sm text-slate-500">
            O pedido vai para a administradora{" "}
            <strong>carolfreitasmuniz@alu.ufc.br</strong>. Você receberá um acesso
            individual após a aprovação.
          </p>

          {erro && (
            <p className="mt-4 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800">
              {erro}
            </p>
          )}
          {ok && (
            <p className="mt-4 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-800">
              {ok}
            </p>
          )}

          <form className="mt-6 space-y-4" onSubmit={onSubmit}>
            <Field
              label="Nome completo"
              value={form.nome}
              onChange={(v) => setForm({ ...form, nome: v })}
              required
            />
            <Field
              label="E-mail institucional"
              type="email"
              value={form.email}
              onChange={(v) => setForm({ ...form, email: v })}
              required
            />
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700">Perfil</label>
              <select
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                value={form.perfilSolicitado}
                onChange={(e) => setForm({ ...form, perfilSolicitado: e.target.value })}
              >
                {PERFIS.map((p) => (
                  <option key={p.value} value={p.value}>
                    {p.label}
                  </option>
                ))}
              </select>
            </div>
            <Field
              label="Setor / unidade"
              value={form.setor}
              onChange={(v) => setForm({ ...form, setor: v })}
            />
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700">
                Justificativa
              </label>
              <textarea
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                rows={3}
                value={form.justificativa}
                onChange={(e) => setForm({ ...form, justificativa: e.target.value })}
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-lg bg-teal-600 py-2.5 text-sm font-medium text-white hover:bg-teal-700 disabled:opacity-50"
            >
              {loading ? "Enviando..." : "Enviar pedido"}
            </button>
          </form>
        </div>

        <Link href="/login" className="mt-6 block text-center text-sm text-slate-500 hover:text-slate-900">
          Já tenho acesso — entrar
        </Link>
      </div>
    </div>
  );
}

function Field({
  label,
  value,
  onChange,
  required,
  type = "text",
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  required?: boolean;
  type?: string;
}) {
  return (
    <div>
      <label className="mb-1.5 block text-sm font-medium text-slate-700">{label}</label>
      <input
        type={type}
        required={required}
        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-teal-500 focus:outline-none focus:ring-2 focus:ring-teal-500/20"
        value={value}
        onChange={(e) => onChange(e.target.value)}
      />
    </div>
  );
}
