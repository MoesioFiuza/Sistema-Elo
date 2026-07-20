using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Elo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Elo.Infrastructure.Data;

public static class DataSeeder
{
    public const string SenhaPadrao = "Elo@123";

    public static async Task SeedAsync(EloDbContext db)
    {
        var hasher = new PasswordHasher<Usuario>();

        if (!await db.Usuarios.AnyAsync())
        {
            var usuarios = new[]
            {
                CriarUsuario(hasher, "Dra. Ana Silva", "medico@elo.local", PerfilUsuario.Medico),
                CriarUsuario(hasher, "Lab. Carlos Mendes", "lab@elo.local", PerfilUsuario.Laboratorio),
                CriarUsuario(hasher, "Enf. CCIH Paula", "ccih@elo.local", PerfilUsuario.CCIH),
                CriarUsuario(hasher, "Enf. Maria Souza", "enfermagem@elo.local", PerfilUsuario.Enfermagem),
                CriarUsuario(hasher, "Admin Sistema", "admin@elo.local", PerfilUsuario.Admin),
            };

            db.Usuarios.AddRange(usuarios);

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
            await db.SaveChangesAsync();
            return;
        }

        // Atualiza hashes legados "dev-only" e garante usuário enfermagem
        var legado = await db.Usuarios.Where(u => u.SenhaHash == "dev-only").ToListAsync();
        foreach (var u in legado)
            u.SenhaHash = hasher.HashPassword(u, SenhaPadrao);

        if (!await db.Usuarios.AnyAsync(u => u.Email == "enfermagem@elo.local"))
        {
            db.Usuarios.Add(CriarUsuario(hasher, "Enf. Maria Souza", "enfermagem@elo.local", PerfilUsuario.Enfermagem));
        }

        if (legado.Count > 0 || db.ChangeTracker.HasChanges())
            await db.SaveChangesAsync();
    }

    private static Usuario CriarUsuario(
        PasswordHasher<Usuario> hasher,
        string nome,
        string email,
        PerfilUsuario perfil)
    {
        var u = new Usuario
        {
            Nome = nome,
            Email = email,
            Perfil = perfil,
            Ativo = true,
        };
        u.SenhaHash = hasher.HashPassword(u, SenhaPadrao);
        return u;
    }
}
