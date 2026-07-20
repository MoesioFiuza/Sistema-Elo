using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Pacientes;

public record CreatePacienteRequest(
    string NumeroProntuario,
    string Nome,
    DateOnly? DataNascimento,
    Sexo Sexo,
    SimNaoNaoRegistrado HistoricoDiarreiaPrevia = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado HistoricoCdiff = SimNaoNaoRegistrado.NaoRegistrado,
    SimNaoNaoRegistrado HistoricoCovid = SimNaoNaoRegistrado.NaoRegistrado,
    string Enfermaria = "",
    string? Leito = null);
