"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { clearAuth, getAuth, type AuthUser, type PerfilUsuario } from "@/lib/auth";
import { useEffect, useState } from "react";
import { BrandMark } from "@/components/brand/BrandMark";

const NAV = [
  { href: "/medico", label: "Médico", perfis: ["Medico", "Admin"] as PerfilUsuario[] },
  { href: "/laboratorio", label: "Laboratório", perfis: ["Laboratorio", "Admin"] as PerfilUsuario[] },
  { href: "/ccih", label: "CCIH", perfis: ["CCIH", "Enfermagem", "Admin"] as PerfilUsuario[] },
  { href: "/pesquisa", label: "Pesquisa", perfis: ["CCIH", "Medico", "Admin"] as PerfilUsuario[] },
  { href: "/admin", label: "Admin", perfis: ["Admin"] as PerfilUsuario[] },
];

type HeaderProps = {
  showLogin?: boolean;
  backHref?: string;
};

export function Header({ showLogin = true, backHref }: HeaderProps) {
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    setUser(getAuth());
  }, [pathname]);

  function sair() {
    clearAuth();
    setUser(null);
    router.push("/login");
  }

  function podeAcessar(perfis: PerfilUsuario[]) {
    if (!user) return false;
    return perfis.includes(user.perfil) || user.perfil === "Admin";
  }

  return (
    <header className="sticky top-0 z-50 border-b border-[var(--elo-border)] bg-white/90 backdrop-blur-md">
      <div className="mx-auto flex max-w-7xl flex-col gap-0 px-4 sm:px-6">
        <div className="flex h-14 items-center justify-between">
          <div className="flex items-center gap-4">
            {backHref && (
              <Link
                href={backHref}
                className="text-sm text-[var(--elo-muted)] transition hover:text-[var(--elo-ink)]"
              >
                Início
              </Link>
            )}
            <BrandMark />
          </div>
          <div className="flex items-center gap-3">
            {user ? (
              <>
                <span className="hidden text-sm text-[var(--elo-muted)] md:inline">
                  {user.nome}
                  <span className="mx-1.5 text-slate-300">·</span>
                  <span className="font-medium text-teal-700">{user.perfil}</span>
                </span>
                <button
                  type="button"
                  onClick={sair}
                  className="rounded-lg border border-[var(--elo-border)] px-3 py-1.5 text-sm text-[var(--elo-ink)] transition hover:bg-slate-50"
                >
                  Sair
                </button>
              </>
            ) : (
              showLogin && (
                <Link
                  href="/login"
                  className="rounded-lg bg-teal-600 px-4 py-2 text-sm font-medium text-white shadow-sm transition hover:bg-teal-700"
                >
                  Entrar
                </Link>
              )
            )}
          </div>
        </div>

        {user && (
          <nav className="-mx-1 flex gap-1 overflow-x-auto pb-3">
            {NAV.map((item) => {
              const ativo = pathname === item.href || pathname.startsWith(item.href + "/");
              const ok = podeAcessar(item.perfis);
              if (!ok) return null;
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={`whitespace-nowrap rounded-full px-3.5 py-1.5 text-sm font-medium transition ${
                    ativo
                      ? "bg-teal-600 text-white shadow-sm"
                      : "text-[var(--elo-muted)] hover:bg-teal-50 hover:text-teal-800"
                  }`}
                >
                  {item.label}
                </Link>
              );
            })}
            <Link
              href="/"
              className={`ml-auto whitespace-nowrap rounded-full px-3.5 py-1.5 text-sm font-medium ${
                pathname === "/"
                  ? "bg-slate-900 text-white"
                  : "text-[var(--elo-muted)] hover:bg-slate-100"
              }`}
            >
              Painel
            </Link>
          </nav>
        )}
      </div>
    </header>
  );
}
