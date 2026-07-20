using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; }

    public Guid? UsuarioDestinoId { get; set; }
    public Usuario? UsuarioDestino { get; set; }

    public PerfilUsuario? PerfilDestino { get; set; }
    public TipoNotificacao Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;

    public Guid? SolicitacaoExameId { get; set; }
    public SolicitacaoExame? SolicitacaoExame { get; set; }

    public bool Lida { get; set; }
    public DateTime? LidaEm { get; set; }
}
