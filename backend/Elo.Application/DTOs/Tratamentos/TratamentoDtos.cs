using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Tratamentos;

public record TratamentoDto(
    Guid Id,
    Guid SolicitacaoExameId,
    string IdAmostraUnico,
    string PacienteNome,
    SimNaoNaoRegistrado IniciouTratamento,
    DateTime? DataInicioTratamento,
    string? Medicacao,
    string? Dose,
    int? DuracaoDias,
    RespostaClinica RespostaDia7,
    RespostaClinica RespostaFinal,
    SimNaoNaoRegistrado Recidiva,
    DateTime? DataRecidiva,
    string? ObservacoesTratamento,
    string? CepaIdentificada);

public record UpsertTratamentoRequest(
    Guid SolicitacaoExameId,
    SimNaoNaoRegistrado IniciouTratamento = SimNaoNaoRegistrado.Sim,
    DateTime? DataInicioTratamento = null,
    string? Medicacao = null,
    string? Dose = null,
    int? DuracaoDias = null,
    RespostaClinica RespostaDia7 = RespostaClinica.NaoRegistrado,
    RespostaClinica RespostaFinal = RespostaClinica.NaoRegistrado,
    SimNaoNaoRegistrado Recidiva = SimNaoNaoRegistrado.NaoRegistrado,
    DateTime? DataRecidiva = null,
    string? ObservacoesTratamento = null);

public record AltaInternacaoRequest(DateTime? DataAlta = null, bool Obito = false);

public record CepaDesfechoDto(
    string Cepa,
    int Total,
    int ComMelhora,
    int ComRecidiva,
    int ComObito);
