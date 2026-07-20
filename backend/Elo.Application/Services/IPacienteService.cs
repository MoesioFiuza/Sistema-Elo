using Elo.Application.DTOs.Pacientes;

namespace Elo.Application.Services;

public interface IPacienteService
{
    Task<IReadOnlyList<PacienteDto>> BuscarAsync(string? termo, CancellationToken ct = default);
    Task<PacienteDetalheDto> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<PacienteDetalheDto> CriarAsync(CreatePacienteRequest request, CancellationToken ct = default);
}
