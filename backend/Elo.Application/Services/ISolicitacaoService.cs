using Elo.Application.DTOs.Solicitacoes;
using Elo.Domain.Enums;

namespace Elo.Application.Services;

public interface ISolicitacaoService
{
    Task<IReadOnlyList<SolicitacaoDto>> ListarAsync(
        StatusSolicitacao? status,
        Guid? pacienteId,
        CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoDto>> ListarFilaLabAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SolicitacaoDto>> ListarHistoricoLabAsync(CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> CriarAsync(CreateSolicitacaoRequest request, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> RegistrarColetaAsync(Guid id, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> AvaliarAmostraAsync(Guid id, AvaliarAmostraRequest request, CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> RegistrarResultadoAsync(
        Guid id,
        RegistrarResultadoRequest request,
        CancellationToken ct = default);
    Task<SolicitacaoDetalheDto> AnexarLaudoAsync(
        Guid id,
        string nomeArquivo,
        string contentType,
        byte[] bytes,
        CancellationToken ct = default);
    Task<LaudoDto> ObterLaudoAsync(Guid id, CancellationToken ct = default);
    Task<(string Nome, string ContentType, byte[] Bytes)> BaixarAnexoAsync(Guid id, CancellationToken ct = default);
}
