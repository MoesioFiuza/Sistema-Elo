"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import { Header } from "@/components/layout/Header";
import { api } from "@/lib/api/client";
import { homeForPerfil, setAuth, type PerfilUsuario } from "@/lib/auth";

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("medico@elo.local");
  const [senha, setSenha] = useState("Elo@123");
  const [erro, setErro] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setLoading(true);
    try {
      const res = await api.auth.login(email, senha);
      setAuth({
        token: res.token,
        expiraEm: res.expiraEm,
        usuarioId: res.usuarioId,
        nome: res.nome,
        email: res.email,
        perfil: res.perfil as PerfilUsuario,
      });
      router.push(homeForPerfil(res.perfil as PerfilUsuario));
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Falha no login");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <Header showLogin={false} backHref="/" />

      <div className="mx-auto flex max-w-sm flex-col px-4 py-16 sm:px-6">
        <div className="rounded-xl border border-slate-200 bg-white p-8 shadow-sm">
          <h1 className="text-xl font-bold text-slate-900">Acesso ao sistema</h1>
          <p className="mt-2 text-sm text-slate-500">
            Credenciais institucionais
          </p>

          {erro && (
            <p className="mt-4 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800">
              {erro}
            </p>
          )}

          <form className="mt-6 space-y-4" onSubmit={onSubmit}>
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700" htmlFor="email">
                E-mail
              </label>
              <input
                id="email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-teal-500 focus:outline-none focus:ring-2 focus:ring-teal-500/20"
              />
            </div>
            <div>
              <label className="mb-1.5 block text-sm font-medium text-slate-700" htmlFor="senha">
                Senha
              </label>
              <input
                id="senha"
                type="password"
                required
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-teal-500 focus:outline-none focus:ring-2 focus:ring-teal-500/20"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-lg bg-teal-600 py-2.5 text-sm font-medium text-white hover:bg-teal-700 disabled:opacity-50"
            >
              {loading ? "Entrando..." : "Entrar"}
            </button>
          </form>

          <div className="mt-6 rounded-lg bg-slate-50 p-3 text-xs text-slate-500">
            <p className="font-medium text-slate-700">Usuários de teste</p>
            <p className="mt-1">medico@elo.local · lab@elo.local · ccih@elo.local</p>
            <p>enfermagem@elo.local · Senha: Elo@123</p>
          </div>
        </div>

        <Link href="/" className="mt-6 text-center text-sm text-slate-500 hover:text-slate-900">
          Voltar
        </Link>
      </div>
    </div>
  );
}
