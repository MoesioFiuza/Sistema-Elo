using Elo.Application.DTOs.Notificacoes;
using Elo.Domain.Enums;

namespace Elo.Application.Services;

public interface INotificacaoService
{
    Task<IReadOnlyList<NotificacaoDto>> ListarAsync(Guid? usuarioId, PerfilUsuario? perfil, bool apenasNaoLidas, CancellationToken ct = default);
    Task MarcarLidaAsync(Guid id, CancellationToken ct = default);
}
