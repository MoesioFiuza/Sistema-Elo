using Elo.Application.DTOs.Tratamentos;

namespace Elo.Application.Services;

public interface ITratamentoService
{
    Task<IReadOnlyList<TratamentoDto>> ListarAsync(CancellationToken ct = default);
    Task<TratamentoDto> UpsertAsync(UpsertTratamentoRequest request, CancellationToken ct = default);
    Task AltaAsync(Guid internacaoId, AltaInternacaoRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CepaDesfechoDto>> CepaVsDesfechoAsync(CancellationToken ct = default);
}
