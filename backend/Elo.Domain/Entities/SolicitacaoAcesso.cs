using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class SolicitacaoAcesso : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario PerfilSolicitado { get; set; }
    public string? Setor { get; set; }
    public string? Justificativa { get; set; }
    public StatusSolicitacaoAcesso Status { get; set; } = StatusSolicitacaoAcesso.Pendente;
    public string? MotivoRecusa { get; set; }

    public Guid? RevisadoPorId { get; set; }
    public Usuario? RevisadoPor { get; set; }
    public DateTime? RevisadoEm { get; set; }

    public Guid? UsuarioCriadoId { get; set; }
    public Usuario? UsuarioCriado { get; set; }
}
