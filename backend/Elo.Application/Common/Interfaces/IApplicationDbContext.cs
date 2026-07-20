using Elo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Paciente> Pacientes { get; }
    DbSet<Internacao> Internacoes { get; }
    DbSet<SolicitacaoExame> SolicitacoesExame { get; }
    DbSet<FormularioClinico> FormulariosClinicos { get; }
    DbSet<ResultadoLaboratorial> ResultadosLaboratoriais { get; }
    DbSet<TratamentoCdiff> TratamentosCdiff { get; }
    DbSet<AuditoriaLog> AuditoriaLogs { get; }
    DbSet<Notificacao> Notificacoes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
