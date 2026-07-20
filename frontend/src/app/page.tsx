"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Header } from "@/components/layout/Header";
import { api } from "@/lib/api/client";
import { getAuth, homeForPerfil, type AuthUser, type PerfilUsuario } from "@/lib/auth";
import type { Paciente } from "@/lib/api/types";
import { Spinner } from "@/components/ui/Spinner";

const MODULOS: {
  titulo: string;
  descricao: string;
  href: string;
  indice: string;
  cor: string;
  perfis: PerfilUsuario[];
}[] = [
  {
    titulo: "Médico",
    descricao: "Admissão e solicitação",
    href: "/medico",
    indice: "01",
    cor: "from-teal-500 to-emerald-600",
    perfis: ["Medico", "Admin"],
  },
  {
    titulo: "Laboratório",
    descricao: "Fila e resultados",
    href: "/laboratorio",
    indice: "02",
    cor: "from-blue-500 to-indigo-600",
    perfis: ["Laboratorio", "Admin"],
  },
  {
    titulo: "CCIH",
    descricao: "Alertas e isolamento",
    href: "/ccih",
    indice: "03",
    cor: "from-amber-500 to-orange-600",
    perfis: ["CCIH", "Enfermagem", "Admin"],
  },
  {
    titulo: "Pesquisa",
    descricao: "Desfecho e análises",
    href: "/pesquisa",
    indice: "04",
    cor: "from-violet-500 to-purple-600",
    perfis: ["CCIH", "Medico", "Admin"],
  },
];

export default function Home() {
  const router = useRouter();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [pacientes, setPacientes] = useState<Paciente[]>([]);
  const [loadingPacientes, setLoadingPacientes] = useState(false);
  const [aviso, setAviso] = useState<string | null>(null);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    setUser(getAuth());
  }, []);

  useEffect(() => {
    if (!user) return;
    if (user.perfil !== "Medico" && user.perfil !== "Admin") return;

    setLoadingPacientes(true);
    api.pacientes
      .buscar()
      .then(setPacientes)
      .catch(() => setPacientes([]))
      .finally(() => setLoadingPacientes(false));
  }, [user]);

  function podeAcessar(perfis: PerfilUsuario[]) {
    if (!user) return false;
    return perfis.includes(user.perfil) || user.perfil === "Admin";
  }

  function abrirModulo(href: string, perfis: PerfilUsuario[]) {
    setAviso(null);
    if (!user) {
      router.push(`/login?next=${encodeURIComponent(href)}`);
      return;
    }
    if (!podeAcessar(perfis)) {
      const nome = MODULOS.find((m) => m.href === href)?.titulo ?? "este módulo";
      setAviso(
        `Seu perfil (${user.perfil}) não acessa ${nome}. Faça logout e entre com o usuário correto.`,
      );
      return;
    }
    router.push(href);
  }

  function abrirPaciente(id: string) {
    router.push(`/medico?paciente=${id}`);
  }

  if (!mounted) {
    return (
      <div className="min-h-screen">
        <Header />
        <div className="flex justify-center py-20">
          <Spinner />
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      <Header />

      <main className="mx-auto max-w-7xl px-4 pb-16 pt-8 sm:px-6">
        <section className="animate-in mb-8 flex flex-wrap items-end justify-between gap-4">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.15em] text-teal-700">
              Painel
            </p>
            <h1 className="mt-1 text-3xl font-bold tracking-tight text-[var(--elo-ink)]">
              {user ? `Olá, ${user.nome.split(" ")[0]}` : "Sistema Elo"}
            </h1>
            <p className="mt-2 max-w-xl text-sm text-[var(--elo-muted)]">
              {user
                ? "Escolha um módulo abaixo. Médicos veem os pacientes cadastrados para acesso rápido."
                : "Entre com suas credenciais para acessar os módulos hospitalares."}
            </p>
          </div>
          {!user && (
            <Link
              href="/login"
              className="rounded-xl bg-teal-600 px-5 py-2.5 text-sm font-semibold text-white shadow-md shadow-teal-600/20 hover:bg-teal-700"
            >
              Entrar no sistema
            </Link>
          )}
          {user && (
            <Link
              href={homeForPerfil(user.perfil)}
              className="rounded-xl border border-[var(--elo-border)] bg-white px-5 py-2.5 text-sm font-medium text-[var(--elo-ink)] hover:border-teal-300"
            >
              Ir ao meu módulo →
            </Link>
          )}
        </section>

        {aviso && (
          <div className="animate-in mb-6 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            {aviso}
            <Link href="/login" className="ml-2 font-semibold text-teal-700 underline">
              Trocar usuário
            </Link>
          </div>
        )}

        {/* Módulos em linha */}
        <section className="animate-in mb-10">
          <h2 className="mb-3 text-xs font-semibold uppercase tracking-wider text-[var(--elo-muted)]">
            Módulos
          </h2>
          <div className="grid grid-cols-2 gap-3 lg:grid-cols-4">
            {MODULOS.map((m) => {
              const ok = !user || podeAcessar(m.perfis);
              return (
                <button
                  key={m.href}
                  type="button"
                  onClick={() => abrirModulo(m.href, m.perfis)}
                  className={`group relative overflow-hidden rounded-2xl border bg-white p-4 text-left shadow-sm transition ${
                    ok
                      ? "border-[var(--elo-border)] hover:-translate-y-0.5 hover:border-teal-300 hover:shadow-md"
                      : "border-slate-100 opacity-60"
                  }`}
                >
                  <div
                    className={`mb-3 flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br ${m.cor} text-sm font-bold text-white shadow`}
                  >
                    {m.indice}
                  </div>
                  <p className="text-xs font-medium text-[var(--elo-muted)]">{m.titulo}</p>
                  <p className="mt-0.5 font-semibold text-[var(--elo-ink)] group-hover:text-teal-800">
                    {m.descricao}
                  </p>
                  {!user && (
                    <p className="mt-2 text-[10px] text-slate-400">Requer login</p>
                  )}
                  {user && !ok && (
                    <p className="mt-2 text-[10px] text-amber-600">Sem permissão</p>
                  )}
                </button>
              );
            })}
          </div>
        </section>

        {/* Lista de pacientes para médico */}
        {user && (user.perfil === "Medico" || user.perfil === "Admin") && (
          <section className="animate-in">
            <div className="mb-3 flex items-end justify-between">
              <div>
                <h2 className="text-xs font-semibold uppercase tracking-wider text-[var(--elo-muted)]">
                  Pacientes cadastrados
                </h2>
                <p className="mt-1 text-sm text-[var(--elo-muted)]">
                  Clique para abrir a solicitação de exame
                </p>
              </div>
              <Link
                href="/medico"
                className="text-sm font-medium text-teal-700 hover:underline"
              >
                Ver módulo médico
              </Link>
            </div>

            <div className="overflow-hidden rounded-2xl border border-[var(--elo-border)] bg-white shadow-sm">
              {loadingPacientes ? (
                <div className="flex justify-center py-12">
                  <Spinner label="Carregando pacientes..." />
                </div>
              ) : pacientes.length === 0 ? (
                <p className="px-6 py-12 text-center text-sm text-slate-500">
                  Nenhum paciente cadastrado ainda.
                </p>
              ) : (
                <ul className="divide-y divide-slate-100">
                  {pacientes.map((p) => (
                    <li key={p.id}>
                      <button
                        type="button"
                        onClick={() => abrirPaciente(p.id)}
                        className="flex w-full items-center justify-between gap-4 px-5 py-4 text-left transition hover:bg-teal-50/60"
                      >
                        <div className="min-w-0">
                          <p className="truncate font-semibold text-[var(--elo-ink)]">
                            {p.nome}
                          </p>
                          <p className="mt-0.5 font-mono text-sm text-teal-700">
                            Prontuário {p.numeroProntuario}
                          </p>
                        </div>
                        <span className="shrink-0 rounded-full bg-teal-600 px-3 py-1.5 text-xs font-semibold text-white">
                          Solicitar exame
                        </span>
                      </button>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </section>
        )}

        {user && user.perfil === "Laboratorio" && (
          <section className="animate-in rounded-2xl border border-blue-100 bg-blue-50/50 p-6">
            <h2 className="font-semibold text-slate-900">Fila do laboratório</h2>
            <p className="mt-1 text-sm text-slate-600">
              Confirme recebimentos e lance resultados (toxina / cepa).
            </p>
            <Link
              href="/laboratorio"
              className="mt-4 inline-flex rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              Abrir fila
            </Link>
          </section>
        )}

        {user && (user.perfil === "CCIH" || user.perfil === "Enfermagem") && (
          <section className="animate-in rounded-2xl border border-amber-100 bg-amber-50/50 p-6">
            <h2 className="font-semibold text-slate-900">Vigilância CCIH</h2>
            <p className="mt-1 text-sm text-slate-600">
              Acompanhe isolamentos e alertas em tempo quase real.
            </p>
            <Link
              href="/ccih"
              className="mt-4 inline-flex rounded-lg bg-amber-600 px-4 py-2 text-sm font-medium text-white hover:bg-amber-700"
            >
              Abrir painel
            </Link>
          </section>
        )}
      </main>
    </div>
  );
}
