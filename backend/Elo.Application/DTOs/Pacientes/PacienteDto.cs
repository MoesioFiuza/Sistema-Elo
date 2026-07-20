using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Pacientes;

public record PacienteDto(
    Guid Id,
    string NumeroProntuario,
    string Nome,
    DateOnly? DataNascimento,
    Sexo Sexo,
    SimNaoNaoRegistrado HistoricoDiarreiaPrevia,
    SimNaoNaoRegistrado HistoricoCdiff,
    DateTime CriadoEm);

public record PacienteDetalheDto(
    Guid Id,
    string NumeroProntuario,
    string Nome,
    DateOnly? DataNascimento,
    Sexo Sexo,
    SimNaoNaoRegistrado HistoricoDiarreiaPrevia,
    SimNaoNaoRegistrado HistoricoCdiff,
    SimNaoNaoRegistrado HistoricoCovid,
    SimNaoNaoRegistrado HistoricoTransplante,
    SimNaoNaoRegistrado HistoricoQuimioterapia,
    IReadOnlyList<InternacaoResumoDto> Internacoes);

public record InternacaoResumoDto(
    Guid Id,
    string Enfermaria,
    string? Leito,
    DateTime DataInternacao,
    bool Ativa);
