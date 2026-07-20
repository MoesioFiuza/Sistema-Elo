using Elo.Application.Common.Interfaces;
using Elo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elo.Infrastructure.Persistence;

public class EloDbContext : DbContext, IApplicationDbContext
{
    public EloDbContext(DbContextOptions<EloDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Internacao> Internacoes => Set<Internacao>();
    public DbSet<SolicitacaoExame> SolicitacoesExame => Set<SolicitacaoExame>();
    public DbSet<FormularioClinico> FormulariosClinicos => Set<FormularioClinico>();
    public DbSet<ResultadoLaboratorial> ResultadosLaboratoriais => Set<ResultadoLaboratorial>();
    public DbSet<TratamentoCdiff> TratamentosCdiff => Set<TratamentoCdiff>();
    public DbSet<AuditoriaLog> AuditoriaLogs => Set<AuditoriaLog>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EloDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var agora = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Domain.Common.EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                entry.Entity.CriadoEm = agora;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.AtualizadoEm = agora;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
