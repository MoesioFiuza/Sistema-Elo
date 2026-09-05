using Elo.Application.Common.Interfaces;
using Elo.Domain.Entities;

namespace Elo.Application.Services;

public class AuditoriaService(IApplicationDbContext db, ICurrentUser currentUser) : IAuditoriaService
{
    public void Registrar(string entidade, Guid entidadeId, string acao, string? dadosNovos = null)
    {
        db.AuditoriaLogs.Add(new AuditoriaLog
        {
            Id = Guid.NewGuid(),
            DataHora = DateTime.UtcNow,
            UsuarioId = currentUser.UsuarioId,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = acao,
            DadosNovos = dadosNovos,
            EnderecoIp = currentUser.EnderecoIp,
        });
    }
}
