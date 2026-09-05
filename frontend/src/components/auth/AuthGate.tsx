"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { getAuth, type AuthUser, type PerfilUsuario } from "@/lib/auth";
import { Spinner } from "@/components/ui/Spinner";

export function useRequireAuth(perfis?: PerfilUsuario[]) {
  const router = useRouter();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [ready, setReady] = useState(false);
  const [negado, setNegado] = useState(false);
  const perfisKey = perfis?.join(",") ?? "";

  useEffect(() => {
    const auth = getAuth();
    if (!auth) {
      router.replace(`/login?next=${encodeURIComponent(window.location.pathname)}`);
      return;
    }
    const allowed = perfisKey ? (perfisKey.split(",") as PerfilUsuario[]) : null;
    if (allowed && !allowed.includes(auth.perfil) && auth.perfil !== "Admin") {
      setNegado(true);
      setUser(auth);
      setReady(true);
      return;
    }
    setNegado(false);
    setUser(auth);
    setReady(true);
  }, [router, perfisKey]);

  return { user, ready, negado };
}

export function AuthGate({
  children,
  perfis,
}: {
  children: React.ReactNode;
  perfis?: PerfilUsuario[];
}) {
  const { ready, negado, user } = useRequireAuth(perfis);

  if (!ready) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <Spinner label="Verificando acesso..." />
      </div>
    );
  }

  if (negado) {
    return (
      <div className="mx-auto max-w-lg animate-in rounded-2xl border border-amber-200 bg-amber-50 p-8 text-center shadow-sm">
        <p className="text-xs font-semibold uppercase tracking-wider text-amber-700">
          Acesso restrito
        </p>
        <h2 className="mt-2 text-xl font-bold text-slate-900">
          Seu perfil não acessa este módulo
        </h2>
        <p className="mt-2 text-sm text-slate-600">
          Você está logado como <strong>{user?.perfil}</strong> ({user?.email}).
          Entre com o usuário do módulo desejado.
        </p>
        <div className="mt-6 flex flex-wrap justify-center gap-3">
          <Link
            href="/login"
            className="rounded-lg bg-teal-600 px-4 py-2 text-sm font-medium text-white hover:bg-teal-700"
          >
            Trocar usuário
          </Link>
          <Link
            href="/"
            className="rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Voltar ao painel
          </Link>
        </div>
        <p className="mt-6 text-xs text-slate-500">
          Desfechos clínicos e pesquisa são restritos à equipe. O laboratório não acessa esses dados.
        </p>
      </div>
    );
  }

  return <>{children}</>;
}
