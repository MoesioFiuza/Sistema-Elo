namespace Elo.Domain.Entities;

public class AuditoriaLog
{
    public Guid Id { get; set; }
    public DateTime DataHora { get; set; }

    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Entidade { get; set; } = string.Empty;
    public Guid EntidadeId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? DadosAnteriores { get; set; }
    public string? DadosNovos { get; set; }
    public string? EnderecoIp { get; set; }
}
