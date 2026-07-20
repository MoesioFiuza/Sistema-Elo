export type Sexo = "NaoInformado" | "Masculino" | "Feminino" | "Outro";
export type SimNao = "NaoRegistrado" | "Sim" | "Nao";
export type TipoCirurgia = "NaoAplicavel" | "Eletiva" | "Urgencia";
export type StatusSolicitacao =
  | "Pendente"
  | "Coletado"
  | "EmAnalise"
  | "ResultadoLiberado"
  | "Cancelado";
export type ResultadoTeste = "NaoRegistrado" | "Positivo" | "Negativo" | "Indeterminado";
export type ConsistenciaFezes = "NaoRegistrado" | "Liquida" | "Pastosa" | "Formada";
export type RespostaClinica =
  | "NaoRegistrado"
  | "Melhora"
  | "SemMelhora"
  | "Piora"
  | "Cura"
  | "Recidiva";
export type TipoNotificacao = "Isolamento" | "Liberacao" | "Resultado" | "Sistema";

export type Paciente = {
  id: string;
  numeroProntuario: string;
  nome: string;
  dataNascimento: string | null;
  sexo: Sexo;
  historicoDiarreiaPrevia: SimNao;
  historicoCdiff: SimNao;
  criadoEm: string;
};

export type PacienteDetalhe = Paciente & {
  historicoCovid: SimNao;
  historicoTransplante: SimNao;
  historicoQuimioterapia: SimNao;
  internacoes: {
    id: string;
    enfermaria: string;
    leito: string | null;
    dataInternacao: string;
    ativa: boolean;
  }[];
};

export type Solicitacao = {
  id: string;
  idAmostraUnico: string;
  status: StatusSolicitacao;
  carimboDataHora: string;
  pacienteNome: string;
  numeroProntuario: string;
  enfermaria: string;
  leito: string | null;
  testeRapido: ResultadoTeste | null;
};

export type SolicitacaoDetalhe = Solicitacao & {
  dataColeta: string | null;
  dataRecebimentoLaboratorio: string | null;
  formularioClinico: Record<string, unknown> | null;
  resultado: {
    dataResultado: string;
    testeRapido: ResultadoTeste;
    toxinaA: ResultadoTeste;
    toxinaB: ResultadoTeste;
    cultura: ResultadoTeste;
    cepaIdentificada: string | null;
    alertaPositivoEnviado: boolean;
  } | null;
};

export type FormularioClinicoPayload = {
  diarreia?: SimNao;
  diasInicioSintomas?: number;
  episodiosDiarreia24h?: number;
  consistenciaFezes?: ConsistenciaFezes;
  sintomasAssociados?: string;
  usoIbpAntesDiarreia?: SimNao;
  usoIbpDuranteDiarreia?: SimNao;
  ibpDescricao?: string;
  dorAbdominal?: SimNao;
  febre?: SimNao;
  temperaturaMaxima?: number;
  duracaoFebre?: string;
  peritonite?: SimNao;
  ventilacaoMecanica?: SimNao;
  internouUtiDurante?: SimNao;
  leucocitose?: SimNao;
  leucopenia?: SimNao;
  fezIra?: SimNao;
  drogasVasoativas?: SimNao;
  desorientacaoConfusao?: SimNao;
  usoAntimicrobianoAntesColeta?: SimNao;
  antimicrobianosAntesDescricao?: string;
  usoAntimicrobianoDiaColeta?: SimNao;
  antimicrobianosDiaColetaDescricao?: string;
  observacoesClinicas?: string;
};

export type InternacaoFichaPayload = {
  motivoInternacao?: string;
  tipoCirurgia?: TipoCirurgia;
  paraTcth?: SimNao;
  paraTos?: SimNao;
  internouComDiarreia?: SimNao;
  usoImunossupressoresDurante?: SimNao;
  usoImunossupressoresAtual?: SimNao;
  imunossupressoresDescricao?: string;
  emUti?: SimNao;
  leucocitose?: SimNao;
  leucopenia?: SimNao;
  sepse?: SimNao;
  obito?: SimNao;
};

export type PacienteHistoricoPayload = {
  diarreiaAssociadaAtbPassado?: SimNao;
  procurouAtendimentoPorDiarreia?: SimNao;
  internadoPorDiarreia?: SimNao;
  quandoInternadoPorDiarreia?: string;
  historicoCdiff?: SimNao;
  cdiffFamiliaAmbiente?: SimNao;
  problemasSaudeAdjacentes?: string;
  problemasSaudeOutros?: string;
  historicoCovid?: SimNao;
  covidAnosPositivos?: string;
  covidTeveSintomas?: SimNao;
  covidSintomasDescricao?: string;
  covidInternado?: SimNao;
  covidDiasInternacao?: number;
  covidOxigenioOuTratamentos?: SimNao;
  covidTratamentosDescricao?: string;
  covidIntubado?: SimNao;
  covidQuandoIntubacao?: string;
  covidDiasIntubado?: number;
  covidUtiDuranteIntubacao?: SimNao;
};

export type DashboardResumo = {
  solicitacoesPendentes: number;
  emAnalise: number;
  resultadosPositivos: number;
  resultadosNegativos: number;
  pacientesComIsolamento: number;
  porEnfermaria: { enfermaria: string; total: number; positivos: number }[];
  alertasRecentes: {
    solicitacaoId: string;
    idAmostraUnico: string;
    pacienteNome: string;
    enfermaria: string;
    dataResultado: string;
    isolamentoAtivo: boolean;
    leito: string | null;
  }[];
};

export type Notificacao = {
  id: string;
  tipo: TipoNotificacao;
  titulo: string;
  mensagem: string;
  solicitacaoExameId: string | null;
  lida: boolean;
  criadoEm: string;
};

export type Tratamento = {
  id: string;
  solicitacaoExameId: string;
  idAmostraUnico: string;
  pacienteNome: string;
  iniciouTratamento: SimNao;
  dataInicioTratamento: string | null;
  medicacao: string | null;
  dose: string | null;
  duracaoDias: number | null;
  respostaDia7: RespostaClinica;
  respostaFinal: RespostaClinica;
  recidiva: SimNao;
  dataRecidiva: string | null;
  observacoesTratamento: string | null;
  cepaIdentificada: string | null;
};

export type CepaDesfecho = {
  cepa: string;
  total: number;
  comMelhora: number;
  comRecidiva: number;
  comObito: number;
};

export type LoginResponse = {
  token: string;
  expiraEm: string;
  usuarioId: string;
  nome: string;
  email: string;
  perfil: string;
};
