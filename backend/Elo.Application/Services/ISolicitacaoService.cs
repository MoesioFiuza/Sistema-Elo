using Elo.Application.DTOs.Solicitacoes;
using Elo.Domain.Enums;

namespace Elo.Application.Services;

public interface ISolicitacaoService
{
    Task<IReadOnlyList<SolicitacaoDto>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoDto>> ListarFilaLabAsync(CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> CriarAsync(CreateSolicitacaoRequest request, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> ConfirmarRecebimentoAsync(Guid id, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> RegistrarResultadoAsync(Guid id, RegistrarResultadoRequest request, CancellationToken ct = default);
}
