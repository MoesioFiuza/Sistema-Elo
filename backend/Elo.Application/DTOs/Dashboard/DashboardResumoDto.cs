namespace Elo.Application.DTOs.Dashboard;

public record DashboardResumoDto(
    int SolicitacoesPendentes,
    int EmAnalise,
    int ResultadosPositivos,
    int ResultadosNegativos,
    int PacientesComIsolamento,
    IReadOnlyList<EnfermariaResumoDto> PorEnfermaria,
    IReadOnlyList<AlertaRecenteDto> AlertasRecentes);

public record EnfermariaResumoDto(string Enfermaria, int Total, int Positivos);

public record AlertaRecenteDto(
    Guid SolicitacaoId,
    string IdAmostraUnico,
    string PacienteNome,
    string Enfermaria,
    DateTime DataResultado,
    bool IsolamentoAtivo,
    string? Leito);
