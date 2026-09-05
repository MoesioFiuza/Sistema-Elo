using Elo.Application.DTOs.Auth;

namespace Elo.Application.Services;

public interface IAcessoService
{
    Task<SolicitarAcessoResponse> SolicitarAsync(SolicitarAcessoRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoAcessoDto>> ListarAsync(CancellationToken ct = default);
    Task<AprovarAcessoResponse> AprovarAsync(Guid id, AprovarAcessoRequest request, CancellationToken ct = default);
    Task RecusarAsync(Guid id, RecusarAcessoRequest request, CancellationToken ct = default);
}
