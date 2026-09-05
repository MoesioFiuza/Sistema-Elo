"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { AuthGate } from "@/components/auth/AuthGate";
import { Spinner } from "@/components/ui/Spinner";
import { api, formatarData, resultadoLabel } from "@/lib/api/client";
import type { Laudo } from "@/lib/api/types";

export default function LaudoPage() {
  return (
    <AuthGate perfis={["Laboratorio", "Medico", "CCIH", "Admin"]}>
      <LaudoContent />
    </AuthGate>
  );
}

function LaudoContent() {
  const params = useParams<{ id: string }>();
  const [laudo, setLaudo] = useState<Laudo | null>(null);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    if (!params.id) return;
    api.solicitacoes
      .laudo(params.id)
      .then(setLaudo)
      .catch((e) => setErro(e instanceof Error ? e.message : "Erro ao carregar laudo"));
  }, [params.id]);

  if (erro) {
    return <p className="p-8 text-center text-sm text-red-700">{erro}</p>;
  }

  if (!laudo) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <Spinner label="Gerando laudo..." />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-100 print:bg-white">
      <div className="mx-auto max-w-3xl p-6 print:p-0">
        <div className="mb-4 flex justify-end gap-2 print:hidden">
          <button
            type="button"
            onClick={() => window.print()}
            className="rounded-lg bg-teal-600 px-4 py-2 text-sm font-medium text-white hover:bg-teal-700"
          >
            Imprimir / salvar PDF
          </button>
          <a
            href="/laboratorio"
            className="rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm"
          >
            Voltar
          </a>
        </div>

        <article className="rounded-xl border border-slate-300 bg-white p-8 shadow-sm print:border-0 print:shadow-none">
          <header className="flex items-start justify-between border-b border-slate-200 pb-4">
            <div>
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src="/nepec-logo.svg" alt="NEPEC" className="h-12 w-auto" />
              <h1 className="mt-3 text-2xl font-bold tracking-tight">Laudo laboratorial</h1>
              <p className="text-sm text-slate-600">
                {laudo.plataforma} · {laudo.laboratorio}
              </p>
            </div>
            <p className="font-mono text-sm font-semibold">{laudo.idAmostraUnico}</p>
          </header>

          <section className="mt-6 grid gap-3 text-sm sm:grid-cols-2">
            <p>
              <strong>Paciente:</strong> {laudo.pacienteNome}
            </p>
            <p>
              <strong>Prontuário:</strong> {laudo.numeroProntuario}
            </p>
            <p>
              <strong>Unidade:</strong> {laudo.enfermaria}
            </p>
            <p>
              <strong>Solicitação:</strong> {formatarData(laudo.carimboDataHora)}
            </p>
            <p>
              <strong>Coleta:</strong> {laudo.dataColeta ? formatarData(laudo.dataColeta) : "—"}
            </p>
            <p>
              <strong>Resultado:</strong> {formatarData(laudo.dataResultado)}
            </p>
          </section>

          <section className="mt-8 grid gap-4 sm:grid-cols-2">
            <div className="rounded-lg border-2 border-slate-800 p-4">
              <p className="text-xs font-bold tracking-wider">RESULTADO DO TESTE RÁPIDO</p>
              <p className="mt-2 text-xl font-extrabold">{resultadoLabel[laudo.testeRapido]}</p>
            </div>
            <div className="rounded-lg border-2 border-slate-800 p-4">
              <p className="text-xs font-bold tracking-wider">RESULTADO DA CULTURA</p>
              <p className="mt-2 text-xl font-extrabold">{resultadoLabel[laudo.cultura]}</p>
            </div>
          </section>

          {laudo.cepaIdentificada && (
            <p className="mt-4 text-sm">
              <strong>Cepa:</strong> {laudo.cepaIdentificada}
            </p>
          )}
          {laudo.observacoesLaboratorio && (
            <p className="mt-2 text-sm">
              <strong>Observações:</strong> {laudo.observacoesLaboratorio}
            </p>
          )}

          <footer className="mt-12 border-t border-slate-200 pt-6">
            {laudo.assinaturaBase64 && (
              // eslint-disable-next-line @next/next/no-img-element
              <img src={laudo.assinaturaBase64} alt="Assinatura" className="h-16 w-auto" />
            )}
            <p className="mt-2 text-sm font-medium">
              {laudo.assinadoPorNome ?? "Responsável laboratorial"}
            </p>
            {laudo.assinadoEm && (
              <p className="text-xs text-slate-500">Assinado em {formatarData(laudo.assinadoEm)}</p>
            )}
          </footer>
        </article>
      </div>
    </div>
  );
}
