"use client";

import Link from "next/link";
import { FormEvent, Suspense, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Header } from "@/components/layout/Header";
import { api } from "@/lib/api/client";
import { homeForPerfil, setAuth, type PerfilUsuario } from "@/lib/auth";

const SHOW_DEMO = process.env.NEXT_PUBLIC_SHOW_DEMO === "true";

export default function LoginPage() {
  return (
    <Suspense>
      <LoginForm />
    </Suspense>
  );
}

function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
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
      const next = searchParams.get("next");
      router.push(next || homeForPerfil(res.perfil as PerfilUsuario));
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Falha no login");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <Header showLogin={false} backHref="/" />

      <div className="mx-auto flex max-w-md flex-col px-4 py-12 sm:px-6">
        <div className="rounded-xl border border-slate-200 bg-white p-8 shadow-sm">
          <p className="text-xs font-semibold uppercase tracking-[0.15em] text-teal-700">
            Cdigital · NEPEC
          </p>
          <h1 className="mt-1 text-xl font-bold text-slate-900">Acesso individual</h1>
          <p className="mt-2 text-sm text-slate-500">
            Cada profissional entra com o próprio e-mail institucional. O acesso não é
            compartilhado por setor — solicite sua conta se ainda não tiver.
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
                autoComplete="username"
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
                autoComplete="current-password"
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

          <p className="mt-6 text-center text-sm text-slate-600">
            Ainda não tem acesso?{" "}
            <Link href="/solicitar-acesso" className="font-semibold text-teal-700 hover:underline">
              Solicitar acesso
            </Link>
          </p>

          {SHOW_DEMO && (
            <div className="mt-6 rounded-lg bg-slate-50 p-3 text-xs text-slate-500">
              <p className="font-medium text-slate-700">Ambiente de desenvolvimento</p>
              <p className="mt-1">medico@elo.local · lab@elo.local · ccih@elo.local</p>
              <p>carolfreitasmuniz@alu.ufc.br · Senha: Elo@123</p>
            </div>
          )}
        </div>

        <Link href="/" className="mt-6 text-center text-sm text-slate-500 hover:text-slate-900">
          Voltar
        </Link>
      </div>
    </div>
  );
}
