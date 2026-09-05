using Elo.Application.Options;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Elo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elo.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        EloDbContext db,
        IOptions<SeedOptions> seedOptions,
        bool isDevelopment,
        ILogger logger)
    {
        var seed = seedOptions.Value;
        var hasher = new PasswordHasher<Usuario>();
        var isDev = isDevelopment;

        var adminEmail = string.IsNullOrWhiteSpace(seed.AdminEmail)
            ? "carolfreitasmuniz@alu.ufc.br"
            : seed.AdminEmail.Trim().ToLowerInvariant();

        var admin = await db.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == adminEmail);
        if (admin is null)
        {
            var senhaAdmin = string.IsNullOrWhiteSpace(seed.AdminPassword)
                ? (isDev ? "Elo@123" : throw new InvalidOperationException(
                    "Defina Seed__AdminPassword para criar a conta administradora em produção."))
                : seed.AdminPassword;

            admin = new Usuario
            {
                Nome = string.IsNullOrWhiteSpace(seed.AdminNome) ? "Carol Freitas Muniz" : seed.AdminNome,
                Email = adminEmail,
                Perfil = PerfilUsuario.Admin,
                Setor = "NEPEC",
                Ativo = true,
            };
            admin.SenhaHash = hasher.HashPassword(admin, senhaAdmin);
            db.Usuarios.Add(admin);
            logger.LogInformation("Administradora criada: {Email}", adminEmail);
        }
        else if (admin.Perfil != PerfilUsuario.Admin)
        {
            admin.Perfil = PerfilUsuario.Admin;
            admin.Ativo = true;
            logger.LogInformation("Perfil de {Email} atualizado para Admin.", adminEmail);
        }

        if (isDev && seed.IncluirUsuariosDemo)
        {
            GarantirUsuario(db, hasher, "Dra. Ana Silva", "medico@elo.local", PerfilUsuario.Medico, "Clínica");
            GarantirUsuario(db, hasher, "Lab. Carlos Mendes", "lab@elo.local", PerfilUsuario.Laboratorio, "Laboratório");
            GarantirUsuario(db, hasher, "Enf. CCIH Paula", "ccih@elo.local", PerfilUsuario.CCIH, "CCIH");
            GarantirUsuario(db, hasher, "Enf. Maria Souza", "enfermagem@elo.local", PerfilUsuario.Enfermagem, "Enfermagem");
        }

        var legado = await db.Usuarios.Where(u => u.SenhaHash == "dev-only").ToListAsync();
        foreach (var u in legado)
            u.SenhaHash = hasher.HashPassword(u, "Elo@123");

        if (isDev && seed.IncluirPacientesDemo && !await db.Pacientes.AnyAsync())
        {
            var paciente1 = new Paciente
            {
                NumeroProntuario = "2024001847",
                Nome = "Maria Oliveira Santos",
                DataNascimento = new DateOnly(1958, 3, 14),
                Sexo = Sexo.Feminino,
                HistoricoDiarreiaPrevia = SimNaoNaoRegistrado.Sim,
                HistoricoCdiff = SimNaoNaoRegistrado.Nao,
            };
            paciente1.Internacoes.Add(new Internacao
            {
                Enfermaria = "Enf. 3A",
                Leito = "12",
                DataInternacao = DateTime.UtcNow.AddDays(-2),
                MotivoInternacao = "Diarreia aguda",
                Leucocitose = SimNaoNaoRegistrado.Sim,
            });

            var paciente2 = new Paciente
            {
                NumeroProntuario = "2024002103",
                Nome = "João Pedro Costa",
                DataNascimento = new DateOnly(1972, 11, 8),
                Sexo = Sexo.Masculino,
                HistoricoCovid = SimNaoNaoRegistrado.Sim,
            };
            paciente2.Internacoes.Add(new Internacao
            {
                Enfermaria = "UTI 2",
                Leito = "04",
                DataInternacao = DateTime.UtcNow.AddDays(-5),
                EmUti = SimNaoNaoRegistrado.Sim,
                Sepse = SimNaoNaoRegistrado.Sim,
            });

            db.Pacientes.AddRange(paciente1, paciente2);
        }

        if (db.ChangeTracker.HasChanges())
            await db.SaveChangesAsync();
    }

    private static void GarantirUsuario(
        EloDbContext db,
        PasswordHasher<Usuario> hasher,
        string nome,
        string email,
        PerfilUsuario perfil,
        string setor)
    {
        if (db.Usuarios.Local.Any(u => u.Email == email))
            return;
        if (db.Usuarios.Any(u => u.Email == email))
            return;

        var u = new Usuario
        {
            Nome = nome,
            Email = email,
            Perfil = perfil,
            Setor = setor,
            Ativo = true,
        };
        u.SenhaHash = hasher.HashPassword(u, "Elo@123");
        db.Usuarios.Add(u);
    }
}
