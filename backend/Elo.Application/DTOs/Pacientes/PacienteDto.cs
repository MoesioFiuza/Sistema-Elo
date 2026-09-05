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
    IReadOnlyList<InternacaoResumoDto> Internacoes,
    IReadOnlyList<ColetaHistoricoDto> ColetasAnteriores);

public record InternacaoResumoDto(
    Guid Id,
    string Enfermaria,
    string? Leito,
    DateTime DataInternacao,
    bool Ativa);

public record ColetaHistoricoDto(
    Guid SolicitacaoId,
    string IdAmostraUnico,
    StatusSolicitacao Status,
    DateTime CarimboDataHora,
    DateTime? DataColeta,
    DateTime? DataResultado,
    ResultadoTeste? TesteRapido,
    ResultadoTeste? Cultura);
