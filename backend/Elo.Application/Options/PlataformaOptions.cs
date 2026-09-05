namespace Elo.Application.Options;

public class PlataformaOptions
{
    public const string SectionName = "Plataforma";

    public string Nome { get; set; } = "Cdigital";
    public string Laboratorio { get; set; } = "NEPEC";
    public string AdminEmail { get; set; } = "carolfreitasmuniz@alu.ufc.br";
    public string FrontendUrl { get; set; } = "http://localhost:3000";
}
