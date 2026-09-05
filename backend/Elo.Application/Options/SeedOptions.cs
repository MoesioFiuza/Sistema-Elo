namespace Elo.Application.Options;

public class SeedOptions
{
    public const string SectionName = "Seed";

    public string AdminEmail { get; set; } = "carolfreitasmuniz@alu.ufc.br";
    public string AdminNome { get; set; } = "Carol Freitas Muniz";
    public string AdminPassword { get; set; } = string.Empty;
    public bool IncluirUsuariosDemo { get; set; } = true;
    public bool IncluirPacientesDemo { get; set; } = true;
}
