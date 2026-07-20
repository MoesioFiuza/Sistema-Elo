using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Notificacoes;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Services;

public class NotificacaoService(IApplicationDbContext db) : INotificacaoService
{
    public async Task<IReadOnlyList<NotificacaoDto>> ListarAsync(
        Guid? usuarioId,
        PerfilUsuario? perfil,
        bool apenasNaoLidas,
        CancellationToken ct = default)
    {
        var query = db.Notificacoes.AsNoTracking().AsQueryable();

        if (usuarioId.HasValue || perfil.HasValue)
        {
            query = query.Where(n =>
                (usuarioId.HasValue && n.UsuarioDestinoId == usuarioId) ||
                (perfil.HasValue && n.PerfilDestino == perfil));
        }

        if (apenasNaoLidas)
            query = query.Where(n => !n.Lida);

        return await query
            .OrderByDescending(n => n.CriadoEm)
            .Take(50)
            .Select(n => new NotificacaoDto(
                n.Id,
                n.Tipo,
                n.Titulo,
                n.Mensagem,
                n.SolicitacaoExameId,
                n.Lida,
                n.CriadoEm))
            .ToListAsync(ct);
    }

    public async Task MarcarLidaAsync(Guid id, CancellationToken ct = default)
    {
        var n = await db.Notificacoes.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Notificação não encontrada.");

        n.Lida = true;
        n.LidaEm = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
