using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Solicitacoes;

public record SolicitacaoDto(
    Guid Id,
    string IdAmostraUnico,
    StatusSolicitacao Status,
    DateTime CarimboDataHora,
    string PacienteNome,
    string NumeroProntuario,
    string Enfermaria,
    string? Leito,
    ResultadoTeste? TesteRapido);

public record SolicitacaoDetalheDto(
    Guid Id,
    string IdAmostraUnico,
    StatusSolicitacao Status,
    DateTime CarimboDataHora,
    DateTime? DataColeta,
    DateTime? DataRecebimentoLaboratorio,
    string PacienteNome,
    string NumeroProntuario,
    string Enfermaria,
    FormularioClinicoDto? FormularioClinico,
    ResultadoLaboratorialDto? Resultado);

public record FormularioClinicoDto(
    SimNaoNaoRegistrado Diarreia,
    int? DiasInicioSintomas,
    int? EpisodiosDiarreia24h,
    ConsistenciaFezes ConsistenciaFezes,
    string? SintomasAssociados,
    SimNaoNaoRegistrado UsoIbpAntesDiarreia,
    SimNaoNaoRegistrado UsoIbpDuranteDiarreia,
    SimNaoNaoRegistrado DorAbdominal,
    SimNaoNaoRegistrado Febre,
    decimal? TemperaturaMaxima,
    string? DuracaoFebre,
    SimNaoNaoRegistrado Peritonite,
    SimNaoNaoRegistrado VentilacaoMecanica,
    SimNaoNaoRegistrado InternouUtiDurante,
    SimNaoNaoRegistrado Leucocitose,
    SimNaoNaoRegistrado Leucopenia,
    SimNaoNaoRegistrado FezIra,
    SimNaoNaoRegistrado DrogasVasoativas,
    SimNaoNaoRegistrado DesorientacaoConfusao,
    SimNaoNaoRegistrado UsoAntimicrobianoAntesColeta,
    string? AntimicrobianosAntesDescricao,
    SimNaoNaoRegistrado UsoAntimicrobianoDiaColeta,
    string? AntimicrobianosDiaColetaDescricao,
    string? ObservacoesClinicas);

public record ResultadoLaboratorialDto(
    DateTime DataResultado,
    ResultadoTeste TesteRapido,
    ResultadoTeste ToxinaA,
    ResultadoTeste ToxinaB,
    ResultadoTeste Cultura,
    string? CepaIdentificada,
    bool AlertaPositivoEnviado);

public record CreateSolicitacaoRequest(
    Guid PacienteId,
    Guid InternacaoId,
    FormularioClinicoInput Formulario,
    InternacaoFichaInput? Internacao = null,
    PacienteHistoricoInput? HistoricoPaciente = null);

public record InternacaoFichaInput(
    string? MotivoInternacao = null,
    TipoCirurgia TipoCirurgia = TipoCirurgia.NaoAplicavel,
    SimNaoNaoRegistrado ParaTcth = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado ParaTos = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado InternouComDiarreia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado UsoImunossupressoresDurante = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado UsoImunossupressoresAtual = SimNaoNaoRegistrado.NaoRegistrado,
    string? ImunossupressoresDescricao = null,
    SimNaoNaoRegistrado EmUti = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Leucocitose = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Leucopenia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Sepse = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Obito = SimNaoNaoRegistrado.NaoRegistrado);

public record PacienteHistoricoInput(
    SimNaoNaoRegistrado DiarreiaAssociadaAtbPassado = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado ProcurouAtendimentoPorDiarreia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado InternadoPorDiarreia = SimNaoNaoRegistrado.NaoRegistrado,
    string? QuandoInternadoPorDiarreia = null,
    SimNaoNaoRegistrado HistoricoCdiff = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado CdiffFamiliaAmbiente = SimNaoNaoRegistrado.NaoRegistrado,
    string? ProblemasSaudeAdjacentes = null,
    string? ProblemasSaudeOutros = null,
    SimNaoNaoRegistrado HistoricoCovid = SimNaoNaoRegistrado.NaoRegistrado,
    string? CovidAnosPositivos = null,
    SimNaoNaoRegistrado CovidTeveSintomas = SimNaoNaoRegistrado.NaoRegistrado,
    string? CovidSintomasDescricao = null,
    SimNaoNaoRegistrado CovidInternado = SimNaoNaoRegistrado.NaoRegistrado,
    int? CovidDiasInternacao = null,
    SimNaoNaoRegistrado CovidOxigenioOuTratamentos = SimNaoNaoRegistrado.NaoRegistrado,
    string? CovidTratamentosDescricao = null,
    SimNaoNaoRegistrado CovidIntubado = SimNaoNaoRegistrado.NaoRegistrado,
    string? CovidQuandoIntubacao = null,
    int? CovidDiasIntubado = null,
    SimNaoNaoRegistrado CovidUtiDuranteIntubacao = SimNaoNaoRegistrado.NaoRegistrado);

public record FormularioClinicoInput(
    SimNaoNaoRegistrado Diarreia = SimNaoNaoRegistrado.NaoRegistrado,
    int? DiasInicioSintomas = null,
    int? EpisodiosDiarreia24h = null,
    ConsistenciaFezes ConsistenciaFezes = ConsistenciaFezes.NaoRegistrado,
    string? SintomasAssociados = null,
    SimNaoNaoRegistrado UsoIbpAntesDiarreia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado UsoIbpDuranteDiarreia = SimNaoNaoRegistrado.NaoRegistrado,
    string? IbpDescricao = null,
    SimNaoNaoRegistrado DorAbdominal = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Febre = SimNaoNaoRegistrado.NaoRegistrado,
    decimal? TemperaturaMaxima = null,
    string? DuracaoFebre = null,
    SimNaoNaoRegistrado Peritonite = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado VentilacaoMecanica = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado InternouUtiDurante = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Leucocitose = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado Leucopenia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado FezIra = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado DrogasVasoativas = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado DesorientacaoConfusao = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado UsoAntimicrobianoAntesColeta = SimNaoNaoRegistrado.NaoRegistrado,
    string? AntimicrobianosAntesDescricao = null,
    SimNaoNaoRegistrado UsoAntimicrobianoDiaColeta = SimNaoNaoRegistrado.NaoRegistrado,
    string? AntimicrobianosDiaColetaDescricao = null,
    string? ObservacoesClinicas = null);

public record RegistrarResultadoRequest(
    ResultadoTeste TesteRapido,
    ResultadoTeste ToxinaA = ResultadoTeste.NaoRegistrado,
    ResultadoTeste ToxinaB = ResultadoTeste.NaoRegistrado,
    ResultadoTeste Cultura = ResultadoTeste.NaoRegistrado,
    string? CepaIdentificada = null,
    string? ObservacoesLaboratorio = null);
