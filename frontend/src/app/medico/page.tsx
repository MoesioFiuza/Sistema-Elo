"use client";

import { FormEvent, Suspense, useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import { AuthGate } from "@/components/auth/AuthGate";
import { PageLayout } from "@/components/layout/PageLayout";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { api } from "@/lib/api/client";
import type {
  ConsistenciaFezes,
  Paciente,
  PacienteDetalhe,
  SimNao,
  TipoCirurgia,
} from "@/lib/api/types";

const NR: SimNao = "NaoRegistrado";

export default function MedicoPage() {
  return (
    <AuthGate perfis={["Medico"]}>
      <Suspense
        fallback={
          <div className="flex min-h-[40vh] items-center justify-center">
            <Spinner label="Carregando..." />
          </div>
        }
      >
        <MedicoContent />
      </Suspense>
    </AuthGate>
  );
}

function MedicoContent() {
  const searchParams = useSearchParams();
  const [busca, setBusca] = useState("");
  const [pacientes, setPacientes] = useState<Paciente[]>([]);
  const [selecionado, setSelecionado] = useState<PacienteDetalhe | null>(null);
  const [loading, setLoading] = useState(true);
  const [msg, setMsg] = useState<string | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [aba, setAba] = useState<"busca" | "novo" | "solicitar">("busca");
  const [passo, setPasso] = useState<"filtro" | "formulario">("filtro");

  const [novo, setNovo] = useState({
    numeroProntuario: "",
    nome: "",
    enfermaria: "",
    leito: "",
  });

  const [filtro, setFiltro] = useState({
    diarreia: false,
    episodios: "3",
    consistencia: "Liquida" as ConsistenciaFezes,
    emUsoAntibiotico: false,
  });

  const [form, setForm] = useState({
    diasInicioSintomas: "",
    episodiosDiarreia24h: "",
    consistenciaFezes: "Liquida" as ConsistenciaFezes,
    sintomasAssociados: "",
    usoIbpAntesDiarreia: NR,
    usoIbpDuranteDiarreia: NR,
    dorAbdominal: NR,
    peritonite: NR,
    ventilacaoMecanica: NR,
    internouUtiDurante: NR,
    leucocitose: NR,
    leucopenia: NR,
    fezIra: NR,
    drogasVasoativas: NR,
    desorientacaoConfusao: NR,
    usoAntimicrobianoAntesColeta: NR,
    antimicrobianosAntesDescricao: "",
    usoAntimicrobianoDiaColeta: NR,
    antimicrobianosDiaColetaDescricao: "",
    observacoesClinicas: "",
  });

  const [internacao, setInternacao] = useState({
    motivoInternacao: "",
    tipoCirurgia: "NaoAplicavel" as TipoCirurgia,
    paraTcth: NR,
    paraTos: NR,
    internouComDiarreia: NR,
    usoImunossupressoresDurante: NR,
    usoImunossupressoresAtual: NR,
    imunossupressoresDescricao: "",
    emUti: NR,
    leucocitose: NR,
    leucopenia: NR,
    sepse: NR,
    obito: NR,
  });

  const [historico, setHistorico] = useState({
    diarreiaAssociadaAtbPassado: NR,
    procurouAtendimentoPorDiarreia: NR,
    internadoPorDiarreia: NR,
    quandoInternadoPorDiarreia: "",
    historicoCdiff: NR,
    cdiffFamiliaAmbiente: NR,
    problemasSaudeAdjacentes: "",
    problemasSaudeOutros: "",
    historicoCovid: NR,
    covidAnosPositivos: "",
    covidTeveSintomas: NR,
    covidSintomasDescricao: "",
    covidInternado: NR,
    covidDiasInternacao: "",
    covidOxigenioOuTratamentos: NR,
    covidTratamentosDescricao: "",
    covidIntubado: NR,
    covidQuandoIntubacao: "",
    covidDiasIntubado: "",
    covidUtiDuranteIntubacao: NR,
  });

  useEffect(() => {
    void carregarPacientes();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    const id = searchParams.get("paciente");
    if (id) void selecionarPaciente(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams]);

  async function carregarPacientes() {
    setLoading(true);
    setErro(null);
    try {
      setPacientes(await api.pacientes.buscar());
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao buscar pacientes");
      setPacientes([]);
    } finally {
      setLoading(false);
    }
  }

  async function selecionarPaciente(id: string) {
    try {
      setSelecionado(await api.pacientes.obter(id));
      setAba("solicitar");
      setPasso("filtro");
      setErro(null);
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao carregar paciente");
    }
  }

  async function criarPaciente(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setMsg(null);
    try {
      const criado = await api.pacientes.criar({
        numeroProntuario: novo.numeroProntuario,
        nome: novo.nome,
        sexo: "NaoInformado",
        enfermaria: novo.enfermaria,
        leito: novo.leito || undefined,
      });
      setSelecionado(criado);
      setMsg("Paciente cadastrado.");
      setAba("solicitar");
      setPasso("filtro");
      void carregarPacientes();
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao cadastrar");
    }
  }

  function avancarFiltro() {
    const episodios = Number(filtro.episodios);
    if (!filtro.diarreia) {
      setErro("Sem diarreia confirmada — não solicite exame. Use outro fluxo clínico.");
      return;
    }
    if (!Number.isFinite(episodios) || episodios < 3) {
      setErro("Diarreia: informe pelo menos 3 episódios em 24 horas.");
      return;
    }
    if (filtro.consistencia !== "Liquida" && filtro.consistencia !== "Pastosa") {
      setErro("Informe o aspecto das fezes (líquido ou pastoso).");
      return;
    }
    setErro(null);
    setForm((f) => ({
      ...f,
      episodiosDiarreia24h: String(episodios),
      consistenciaFezes: filtro.consistencia,
      usoAntimicrobianoDiaColeta: filtro.emUsoAntibiotico ? "Sim" : f.usoAntimicrobianoDiaColeta,
    }));
    setPasso("formulario");
  }

  async function criarSolicitacao(e: FormEvent) {
    e.preventDefault();
    if (!selecionado) return;
    const internacaoAtiva = selecionado.internacoes.find((i) => i.ativa);
    if (!internacaoAtiva) {
      setErro("Paciente sem internação ativa.");
      return;
    }
    setErro(null);
    setMsg(null);
    try {
      const sol = await api.solicitacoes.criar({
        pacienteId: selecionado.id,
        internacaoId: internacaoAtiva.id,
        formulario: {
          diarreia: "Sim",
          diasInicioSintomas: form.diasInicioSintomas
            ? Number(form.diasInicioSintomas)
            : undefined,
          episodiosDiarreia24h: form.episodiosDiarreia24h
            ? Number(form.episodiosDiarreia24h)
            : undefined,
          consistenciaFezes: form.consistenciaFezes,
          sintomasAssociados: form.sintomasAssociados || undefined,
          usoIbpAntesDiarreia: form.usoIbpAntesDiarreia,
          usoIbpDuranteDiarreia: form.usoIbpDuranteDiarreia,
          dorAbdominal: form.dorAbdominal,
          peritonite: form.peritonite,
          ventilacaoMecanica: form.ventilacaoMecanica,
          internouUtiDurante: form.internouUtiDurante,
          leucocitose: form.leucocitose,
          leucopenia: form.leucopenia,
          fezIra: form.fezIra,
          drogasVasoativas: form.drogasVasoativas,
          desorientacaoConfusao: form.desorientacaoConfusao,
          usoAntimicrobianoAntesColeta: form.usoAntimicrobianoAntesColeta,
          antimicrobianosAntesDescricao:
            form.antimicrobianosAntesDescricao || undefined,
          usoAntimicrobianoDiaColeta: form.usoAntimicrobianoDiaColeta,
          antimicrobianosDiaColetaDescricao:
            form.antimicrobianosDiaColetaDescricao || undefined,
          observacoesClinicas: form.observacoesClinicas || undefined,
        },
        internacao: {
          motivoInternacao: internacao.motivoInternacao || undefined,
          tipoCirurgia: internacao.tipoCirurgia,
          paraTcth: internacao.paraTcth,
          paraTos: internacao.paraTos,
          internouComDiarreia: internacao.internouComDiarreia,
          usoImunossupressoresDurante: internacao.usoImunossupressoresDurante,
          usoImunossupressoresAtual: internacao.usoImunossupressoresAtual,
          imunossupressoresDescricao:
            internacao.imunossupressoresDescricao || undefined,
          emUti: internacao.emUti,
          leucocitose: internacao.leucocitose,
          leucopenia: internacao.leucopenia,
          sepse: internacao.sepse,
          obito: internacao.obito,
        },
        historicoPaciente: {
          ...historico,
          quandoInternadoPorDiarreia:
            historico.quandoInternadoPorDiarreia || undefined,
          problemasSaudeAdjacentes:
            historico.problemasSaudeAdjacentes || undefined,
          problemasSaudeOutros: historico.problemasSaudeOutros || undefined,
          covidAnosPositivos: historico.covidAnosPositivos || undefined,
          covidSintomasDescricao: historico.covidSintomasDescricao || undefined,
          covidDiasInternacao: historico.covidDiasInternacao
            ? Number(historico.covidDiasInternacao)
            : undefined,
          covidTratamentosDescricao:
            historico.covidTratamentosDescricao || undefined,
          covidQuandoIntubacao: historico.covidQuandoIntubacao || undefined,
          covidDiasIntubado: historico.covidDiasIntubado
            ? Number(historico.covidDiasIntubado)
            : undefined,
        },
      });
      setMsg(`Solicitação enviada ao laboratório: ${sol.idAmostraUnico}`);
      setPasso("filtro");
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Erro ao solicitar exame");
    }
  }

  const termo = busca.trim().toLowerCase();
  const pacientesFiltrados = termo
    ? pacientes.filter(
        (p) =>
          p.nome.toLowerCase().includes(termo) ||
          p.numeroProntuario.toLowerCase().includes(termo),
      )
    : pacientes;

  const tabs = [
    { id: "busca" as const, label: "Pacientes" },
    { id: "novo" as const, label: "Novo paciente" },
    { id: "solicitar" as const, label: "Ficha completa" },
  ];

  const internacaoAtiva = selecionado?.internacoes.find((i) => i.ativa);

  return (
    <PageLayout
      badge="Médico"
      title="Admissão e solicitação"
      subtitle="Ficha epidemiológica completa · C. difficile"
    >
      {msg && <Alert variant="success">{msg}</Alert>}
      {erro && <Alert variant="error">{erro}</Alert>}

      <div className="mb-6 flex flex-wrap gap-2">
        {tabs.map((t) => (
          <button
            key={t.id}
            onClick={() => setAba(t.id)}
            className={`rounded-lg px-4 py-2 text-sm font-medium transition ${
              aba === t.id
                ? "bg-teal-600 text-white"
                : "border border-slate-200 bg-white text-slate-600 hover:border-slate-300"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {aba === "busca" && (
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex gap-3">
            <input
              className="flex-1 rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-teal-500 focus:outline-none focus:ring-2 focus:ring-teal-500/20"
              placeholder="Filtrar por nome ou prontuário"
              value={busca}
              onChange={(e) => setBusca(e.target.value)}
            />
            <button
              type="button"
              onClick={() => carregarPacientes()}
              className="rounded-lg border border-slate-200 bg-white px-5 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
            >
              Atualizar
            </button>
          </div>
          {loading ? (
            <Spinner label="Carregando pacientes..." />
          ) : pacientesFiltrados.length === 0 ? (
            <p className="py-8 text-center text-sm text-slate-500">
              {pacientes.length === 0
                ? "Nenhum paciente cadastrado ainda. Use Novo paciente."
                : "Nenhum paciente com esse filtro."}
            </p>
          ) : (
            <ul className="mt-4 divide-y divide-slate-100">
              {pacientesFiltrados.map((p) => (
                <li key={p.id}>
                  <button
                    type="button"
                    className="flex w-full items-center justify-between py-3 text-left hover:bg-slate-50"
                    onClick={() => selecionarPaciente(p.id)}
                  >
                    <div>
                      <p className="font-medium text-slate-900">{p.nome}</p>
                      <p className="text-sm text-slate-500">
                        Prontuário {p.numeroProntuario}
                      </p>
                    </div>
                    <span className="text-sm text-teal-600">Abrir ficha</span>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {aba === "novo" && (
        <form
          onSubmit={criarPaciente}
          className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
          <div className="grid gap-4 sm:grid-cols-2">
            <Field
              label="Prontuário"
              value={novo.numeroProntuario}
              onChange={(v) => setNovo({ ...novo, numeroProntuario: v })}
              required
            />
            <Field
              label="Nome"
              value={novo.nome}
              onChange={(v) => setNovo({ ...novo, nome: v })}
              required
            />
            <Field
              label="Enfermaria"
              value={novo.enfermaria}
              onChange={(v) => setNovo({ ...novo, enfermaria: v })}
              required
            />
            <Field
              label="Leito"
              value={novo.leito}
              onChange={(v) => setNovo({ ...novo, leito: v })}
            />
          </div>
          <button
            type="submit"
            className="rounded-lg bg-teal-600 px-6 py-2 text-sm font-medium text-white hover:bg-teal-700"
          >
            Cadastrar
          </button>
        </form>
      )}

      {aba === "solicitar" && (
        <div className="space-y-4">
          {selecionado ? (
            <div className="rounded-xl border border-teal-100 bg-teal-50/60 p-4">
              <p className="text-xs font-semibold uppercase tracking-wider text-teal-700">
                Paciente
              </p>
              <p className="mt-1 text-lg font-semibold text-slate-900">
                {selecionado.nome}
              </p>
              <p className="text-sm text-slate-600">
                Prontuário {selecionado.numeroProntuario}
                {internacaoAtiva && (
                  <>
                    {" "}
                    · {internacaoAtiva.enfermaria}
                    {internacaoAtiva.leito ? ` · Leito ${internacaoAtiva.leito}` : ""}
                    {" "}
                    · Internação {new Date(internacaoAtiva.dataInternacao).toLocaleDateString("pt-BR")}
                  </>
                )}
              </p>
            </div>
          ) : (
            <p className="rounded-xl border border-slate-200 bg-white p-6 text-sm text-slate-500">
              Selecione ou cadastre um paciente.
            </p>
          )}

          {selecionado && (selecionado.coletasAnteriores?.length ?? 0) > 0 && (
            <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
              <h3 className="text-xs font-semibold uppercase tracking-wider text-teal-700">
                Coletas anteriores deste paciente
              </h3>
              <p className="mt-1 text-xs text-slate-500">
                Uma nova solicitação não apaga resultados antigos.
              </p>
              <ul className="mt-3 divide-y divide-slate-100 text-sm">
                {selecionado.coletasAnteriores?.map((c) => (
                  <li key={c.solicitacaoId} className="flex flex-wrap justify-between gap-2 py-2">
                    <span className="font-mono text-xs font-semibold">{c.idAmostraUnico}</span>
                    <span className="text-slate-500">
                      {new Date(c.carimboDataHora).toLocaleDateString("pt-BR")}
                      {c.testeRapido ? ` · TR ${c.testeRapido}` : ""}
                      {c.cultura ? ` · Cultura ${c.cultura}` : ""}
                    </span>
                  </li>
                ))}
              </ul>
            </div>
          )}

          {selecionado && passo === "filtro" && (
            <div className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
              <h3 className="font-semibold text-slate-900">Checklist clínico</h3>
              <label className="flex items-start gap-2 text-sm">
                <input
                  type="checkbox"
                  className="mt-1"
                  checked={filtro.diarreia}
                  onChange={(e) =>
                    setFiltro({ ...filtro, diarreia: e.target.checked })
                  }
                />
                <span>
                  Diarreia confirmada — pelo menos 3 episódios em 24h, aspecto
                  líquido ou pastoso
                </span>
              </label>
              <div className="grid gap-4 sm:grid-cols-2">
                <Field
                  label="Episódios em 24h (mínimo 3)"
                  value={filtro.episodios}
                  onChange={(v) => setFiltro({ ...filtro, episodios: v })}
                  type="number"
                />
                <div>
                  <label className="mb-1.5 block text-sm font-medium text-slate-700">
                    Aspecto das fezes
                  </label>
                  <select
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                    value={filtro.consistencia}
                    onChange={(e) =>
                      setFiltro({
                        ...filtro,
                        consistencia: e.target.value as ConsistenciaFezes,
                      })
                    }
                  >
                    <option value="Liquida">Líquido</option>
                    <option value="Pastosa">Pastoso</option>
                  </select>
                </div>
              </div>
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={filtro.emUsoAntibiotico}
                  onChange={(e) =>
                    setFiltro({ ...filtro, emUsoAntibiotico: e.target.checked })
                  }
                />
                Em uso de antibiótico
              </label>
              <button
                type="button"
                onClick={avancarFiltro}
                className="rounded-lg bg-teal-600 px-6 py-2 text-sm font-medium text-white hover:bg-teal-700"
              >
                Continuar para ficha completa
              </button>
            </div>
          )}

          {selecionado && passo === "formulario" && (
            <form onSubmit={criarSolicitacao} className="space-y-6">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-semibold text-slate-900">
                  Ficha epidemiológica
                </h3>
                <button
                  type="button"
                  className="text-sm text-teal-600"
                  onClick={() => setPasso("filtro")}
                >
                  Voltar ao filtro
                </button>
              </div>

              <Section title="Internação">
                <Field
                  label="Motivo da internação (doença, cirurgia, TCTH, TOS…)"
                  value={internacao.motivoInternacao}
                  onChange={(v) =>
                    setInternacao({ ...internacao, motivoInternacao: v })
                  }
                />
                <div>
                  <label className="mb-1.5 block text-sm font-medium text-slate-700">
                    Se cirurgia: eletiva ou urgência?
                  </label>
                  <select
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                    value={internacao.tipoCirurgia}
                    onChange={(e) =>
                      setInternacao({
                        ...internacao,
                        tipoCirurgia: e.target.value as TipoCirurgia,
                      })
                    }
                  >
                    <option value="NaoAplicavel">Não aplicável</option>
                    <option value="Eletiva">Eletiva</option>
                    <option value="Urgencia">Urgência</option>
                  </select>
                </div>
                <SelectSimNao
                  label="Para realizar TCTH?"
                  value={internacao.paraTcth}
                  onChange={(v) => setInternacao({ ...internacao, paraTcth: v })}
                />
                <SelectSimNao
                  label="Para realizar TOS?"
                  value={internacao.paraTos}
                  onChange={(v) => setInternacao({ ...internacao, paraTos: v })}
                />
                <SelectSimNao
                  label="Internou com diarreia?"
                  value={internacao.internouComDiarreia}
                  onChange={(v) =>
                    setInternacao({ ...internacao, internouComDiarreia: v })
                  }
                />
                <SelectSimNao
                  label="Uso de imunossupressores durante a internação"
                  value={internacao.usoImunossupressoresDurante}
                  onChange={(v) =>
                    setInternacao({
                      ...internacao,
                      usoImunossupressoresDurante: v,
                    })
                  }
                />
                <SelectSimNao
                  label="Uso atual de imunossupressores?"
                  value={internacao.usoImunossupressoresAtual}
                  onChange={(v) =>
                    setInternacao({
                      ...internacao,
                      usoImunossupressoresAtual: v,
                    })
                  }
                />
                <Field
                  label="Quais imunossupressores"
                  value={internacao.imunossupressoresDescricao}
                  onChange={(v) =>
                    setInternacao({
                      ...internacao,
                      imunossupressoresDescricao: v,
                    })
                  }
                />
              </Section>

              <Section title="Antimicrobianos e sintomas">
                <SelectSimNao
                  label="Uso de antimicrobianos ANTES DA COLETA (durante internação)"
                  value={form.usoAntimicrobianoAntesColeta}
                  onChange={(v) =>
                    setForm({ ...form, usoAntimicrobianoAntesColeta: v })
                  }
                />
                <Field
                  label="Quais ATM (antes da coleta)"
                  value={form.antimicrobianosAntesDescricao}
                  onChange={(v) =>
                    setForm({ ...form, antimicrobianosAntesDescricao: v })
                  }
                />
                <SelectSimNao
                  label="Uso atual de antimicrobianos (DIA DA COLETA)"
                  value={form.usoAntimicrobianoDiaColeta}
                  onChange={(v) =>
                    setForm({ ...form, usoAntimicrobianoDiaColeta: v })
                  }
                />
                <Field
                  label="Quais ATM (dia da coleta)"
                  value={form.antimicrobianosDiaColetaDescricao}
                  onChange={(v) =>
                    setForm({ ...form, antimicrobianosDiaColetaDescricao: v })
                  }
                />
                <Field
                  label="Quando os sintomas começaram? (dias) Ex: 30"
                  value={form.diasInicioSintomas}
                  onChange={(v) => setForm({ ...form, diasInicioSintomas: v })}
                  type="number"
                />
                <Field
                  label="Frequência dos episódios (vezes/dia) Ex: 3"
                  value={form.episodiosDiarreia24h}
                  onChange={(v) =>
                    setForm({ ...form, episodiosDiarreia24h: v })
                  }
                  type="number"
                />
                <div>
                  <label className="mb-1.5 block text-sm font-medium text-slate-700">
                    Consistência das fezes
                  </label>
                  <select
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                    value={form.consistenciaFezes}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        consistenciaFezes: e.target.value as ConsistenciaFezes,
                      })
                    }
                  >
                    <option value="Liquida">Líquida</option>
                    <option value="Pastosa">Pastosa</option>
                    <option value="Formada">Formada</option>
                  </select>
                </div>
                <Field
                  label="Sintomas associados"
                  value={form.sintomasAssociados}
                  onChange={(v) => setForm({ ...form, sintomasAssociados: v })}
                />
                <SelectSimNao
                  label="Uso de IBP antes do início da diarreia"
                  value={form.usoIbpAntesDiarreia}
                  onChange={(v) =>
                    setForm({ ...form, usoIbpAntesDiarreia: v })
                  }
                />
                <SelectSimNao
                  label="Uso de IBP durante diarreia"
                  value={form.usoIbpDuranteDiarreia}
                  onChange={(v) =>
                    setForm({ ...form, usoIbpDuranteDiarreia: v })
                  }
                />
                <SelectSimNao
                  label="Dor abdominal / distensão?"
                  value={form.dorAbdominal}
                  onChange={(v) => setForm({ ...form, dorAbdominal: v })}
                />
              </Section>

              <Section title="Gravidade / complicações">
                <SelectSimNao
                  label="Sinais de peritonite / perfuração intestinal?"
                  value={form.peritonite}
                  onChange={(v) => setForm({ ...form, peritonite: v })}
                />
                <SelectSimNao
                  label="Insuficiência respiratória com necessidade de VM?"
                  value={form.ventilacaoMecanica}
                  onChange={(v) => setForm({ ...form, ventilacaoMecanica: v })}
                />
                <SelectSimNao
                  label="Internou em UTI?"
                  value={form.internouUtiDurante}
                  onChange={(v) => {
                    setForm({ ...form, internouUtiDurante: v });
                    setInternacao({ ...internacao, emUti: v });
                  }}
                />
                <SelectSimNao
                  label="Leucocitose > 15.000?"
                  value={form.leucocitose}
                  onChange={(v) => {
                    setForm({ ...form, leucocitose: v });
                    setInternacao({ ...internacao, leucocitose: v });
                  }}
                />
                <SelectSimNao
                  label="Leucopenia < 4.000?"
                  value={form.leucopenia}
                  onChange={(v) => {
                    setForm({ ...form, leucopenia: v });
                    setInternacao({ ...internacao, leucopenia: v });
                  }}
                />
                <SelectSimNao
                  label="Fez IRA?"
                  value={form.fezIra}
                  onChange={(v) => setForm({ ...form, fezIra: v })}
                />
                <SelectSimNao
                  label="Drogas vasoativas durante a diarreia?"
                  value={form.drogasVasoativas}
                  onChange={(v) => setForm({ ...form, drogasVasoativas: v })}
                />
                <SelectSimNao
                  label="Desorientação, confusão mental ou rebaixamento de sensorio?"
                  value={form.desorientacaoConfusao}
                  onChange={(v) =>
                    setForm({ ...form, desorientacaoConfusao: v })
                  }
                />
                <SelectSimNao
                  label="FEZ SEPSE"
                  value={internacao.sepse}
                  onChange={(v) => setInternacao({ ...internacao, sepse: v })}
                />
                <SelectSimNao
                  label="ÓBITO"
                  value={internacao.obito}
                  onChange={(v) => setInternacao({ ...internacao, obito: v })}
                />
              </Section>

              <Section title="Histórico de diarreia / C. diff">
                <SelectSimNao
                  label="Já teve diarreia associada a ATB no passado?"
                  value={historico.diarreiaAssociadaAtbPassado}
                  onChange={(v) =>
                    setHistorico({ ...historico, diarreiaAssociadaAtbPassado: v })
                  }
                />
                <SelectSimNao
                  label="Já procurou atendimento médico por diarreia?"
                  value={historico.procurouAtendimentoPorDiarreia}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      procurouAtendimentoPorDiarreia: v,
                    })
                  }
                />
                <SelectSimNao
                  label="Já foi internado por diarreia?"
                  value={historico.internadoPorDiarreia}
                  onChange={(v) =>
                    setHistorico({ ...historico, internadoPorDiarreia: v })
                  }
                />
                <Field
                  label="Se sim, quando? (mês/ano ou ano)"
                  value={historico.quandoInternadoPorDiarreia}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      quandoInternadoPorDiarreia: v,
                    })
                  }
                />
                <SelectSimNao
                  label="Já teve infecção por C. diff no passado?"
                  value={historico.historicoCdiff}
                  onChange={(v) =>
                    setHistorico({ ...historico, historicoCdiff: v })
                  }
                />
                <SelectSimNao
                  label="Casos de C. diff na família ou ambiente próximo?"
                  value={historico.cdiffFamiliaAmbiente}
                  onChange={(v) =>
                    setHistorico({ ...historico, cdiffFamiliaAmbiente: v })
                  }
                />
                <Field
                  label="Problemas de saúde que enfraquecem o sistema imunológico"
                  value={historico.problemasSaudeAdjacentes}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      problemasSaudeAdjacentes: v,
                    })
                  }
                />
                <Field
                  label='Se "outros", especifique'
                  value={historico.problemasSaudeOutros}
                  onChange={(v) =>
                    setHistorico({ ...historico, problemasSaudeOutros: v })
                  }
                />
              </Section>

              <Section title="Histórico COVID">
                <SelectSimNao
                  label="Já testou positivo para COVID?"
                  value={historico.historicoCovid}
                  onChange={(v) =>
                    setHistorico({ ...historico, historicoCovid: v })
                  }
                />
                <Field
                  label="Se sim, quando? (ano ou anos) Ex: 2019/2020"
                  value={historico.covidAnosPositivos}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidAnosPositivos: v })
                  }
                />
                <SelectSimNao
                  label="Teve sintomas quando foi diagnosticado?"
                  value={historico.covidTeveSintomas}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidTeveSintomas: v })
                  }
                />
                <Field
                  label="Quais sintomas?"
                  value={historico.covidSintomasDescricao}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidSintomasDescricao: v })
                  }
                />
                <SelectSimNao
                  label="Já foi internado por complicações da COVID?"
                  value={historico.covidInternado}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidInternado: v })
                  }
                />
                <Field
                  label="Quanto tempo no hospital? (dias)"
                  value={historico.covidDiasInternacao}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidDiasInternacao: v })
                  }
                  type="number"
                />
                <SelectSimNao
                  label="Recebeu O₂ suplementar ou outros tratamentos?"
                  value={historico.covidOxigenioOuTratamentos}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      covidOxigenioOuTratamentos: v,
                    })
                  }
                />
                <Field
                  label="Quais tratamentos?"
                  value={historico.covidTratamentosDescricao}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      covidTratamentosDescricao: v,
                    })
                  }
                />
                <SelectSimNao
                  label="Já foi intubado por COVID?"
                  value={historico.covidIntubado}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidIntubado: v })
                  }
                />
                <Field
                  label="Quando foi a intubação? (ano ou mês/ano)"
                  value={historico.covidQuandoIntubacao}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidQuandoIntubacao: v })
                  }
                />
                <Field
                  label="Quanto tempo intubado? (dias)"
                  value={historico.covidDiasIntubado}
                  onChange={(v) =>
                    setHistorico({ ...historico, covidDiasIntubado: v })
                  }
                  type="number"
                />
                <SelectSimNao
                  label="Transferido para UTI durante intubação?"
                  value={historico.covidUtiDuranteIntubacao}
                  onChange={(v) =>
                    setHistorico({
                      ...historico,
                      covidUtiDuranteIntubacao: v,
                    })
                  }
                />
              </Section>

              <Section title="Observações">
                <div className="sm:col-span-2">
                  <label className="mb-1.5 block text-sm font-medium text-slate-700">
                    Observações clínicas
                  </label>
                  <textarea
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
                    rows={3}
                    value={form.observacoesClinicas}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        observacoesClinicas: e.target.value,
                      })
                    }
                  />
                </div>
              </Section>

              <p className="text-xs text-slate-500">
                Tratamento C. diff, resposta no 7º dia / final, recidiva, teste
                rápido e cultura são preenchidos nos módulos Pesquisa e
                Laboratório após o resultado.
              </p>

              <button
                type="submit"
                className="rounded-xl bg-teal-600 px-8 py-3 text-sm font-semibold text-white shadow-sm hover:bg-teal-700"
              >
                Gerar ID de amostra e enviar ao laboratório
              </button>
            </form>
          )}
        </div>
      )}
    </PageLayout>
  );
}

function Section({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <section className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
      <h4 className="mb-4 text-xs font-semibold uppercase tracking-wider text-teal-700">
        {title}
      </h4>
      <div className="grid gap-4 sm:grid-cols-2">{children}</div>
    </section>
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
      <label className="mb-1.5 block text-sm font-medium text-slate-700">
        {label}
      </label>
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

function SelectSimNao({
  label,
  value,
  onChange,
}: {
  label: string;
  value: SimNao;
  onChange: (v: SimNao) => void;
}) {
  return (
    <div>
      <label className="mb-1.5 block text-sm font-medium text-slate-700">
        {label}
      </label>
      <select
        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm"
        value={value}
        onChange={(e) => onChange(e.target.value as SimNao)}
      >
        <option value="NaoRegistrado">Não registrado</option>
        <option value="Sim">Sim</option>
        <option value="Nao">Não</option>
      </select>
    </div>
  );
}
