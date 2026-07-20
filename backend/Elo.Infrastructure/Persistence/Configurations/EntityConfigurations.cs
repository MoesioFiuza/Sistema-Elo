using Elo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elo.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nome).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.SenhaHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(32);
    }
}

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("pacientes");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.NumeroProntuario).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.NumeroProntuario).IsUnique();
        builder.Property(p => p.Nome).HasMaxLength(300).IsRequired();
    }
}

public class InternacaoConfiguration : IEntityTypeConfiguration<Internacao>
{
    public void Configure(EntityTypeBuilder<Internacao> builder)
    {
        builder.ToTable("internacoes");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Enfermaria).HasMaxLength(100).IsRequired();
        builder.Property(i => i.Leito).HasMaxLength(20);
        builder.HasOne(i => i.Paciente)
            .WithMany(p => p.Internacoes)
            .HasForeignKey(i => i.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SolicitacaoExameConfiguration : IEntityTypeConfiguration<SolicitacaoExame>
{
    public void Configure(EntityTypeBuilder<SolicitacaoExame> builder)
    {
        builder.ToTable("solicitacoes_exame");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.IdAmostraUnico).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.IdAmostraUnico).IsUnique();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasOne(s => s.Paciente)
            .WithMany(p => p.Solicitacoes)
            .HasForeignKey(s => s.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Internacao)
            .WithMany(i => i.Solicitacoes)
            .HasForeignKey(s => s.InternacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Solicitante)
            .WithMany(u => u.SolicitacoesRealizadas)
            .HasForeignKey(s => s.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FormularioClinicoConfiguration : IEntityTypeConfiguration<FormularioClinico>
{
    public void Configure(EntityTypeBuilder<FormularioClinico> builder)
    {
        builder.ToTable("formularios_clinicos");
        builder.HasKey(f => f.Id);
        builder.HasOne(f => f.SolicitacaoExame)
            .WithOne(s => s.FormularioClinico)
            .HasForeignKey<FormularioClinico>(f => f.SolicitacaoExameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ResultadoLaboratorialConfiguration : IEntityTypeConfiguration<ResultadoLaboratorial>
{
    public void Configure(EntityTypeBuilder<ResultadoLaboratorial> builder)
    {
        builder.ToTable("resultados_laboratoriais");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.CepaIdentificada).HasMaxLength(100);
        builder.HasOne(r => r.SolicitacaoExame)
            .WithOne(s => s.ResultadoLaboratorial)
            .HasForeignKey<ResultadoLaboratorial>(r => r.SolicitacaoExameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TratamentoCdiffConfiguration : IEntityTypeConfiguration<TratamentoCdiff>
{
    public void Configure(EntityTypeBuilder<TratamentoCdiff> builder)
    {
        builder.ToTable("tratamentos_cdiff");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Medicacao).HasMaxLength(200);
        builder.HasOne(t => t.SolicitacaoExame)
            .WithOne(s => s.TratamentoCdiff)
            .HasForeignKey<TratamentoCdiff>(t => t.SolicitacaoExameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
{
    public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
    {
        builder.ToTable("auditoria_logs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Entidade).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Acao).HasMaxLength(50).IsRequired();
        builder.HasIndex(a => new { a.Entidade, a.EntidadeId });
        builder.HasIndex(a => a.DataHora);
    }
}

public class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
{
    public void Configure(EntityTypeBuilder<Notificacao> builder)
    {
        builder.ToTable("notificacoes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Mensagem).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.Tipo).HasConversion<string>().HasMaxLength(32);
        builder.Property(n => n.PerfilDestino).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(n => n.CriadoEm);
        builder.HasIndex(n => new { n.Lida, n.PerfilDestino });
    }
}
