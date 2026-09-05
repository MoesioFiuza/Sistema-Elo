using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public string? Setor { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<SolicitacaoExame> SolicitacoesRealizadas { get; set; } = [];
    public ICollection<AuditoriaLog> Auditorias { get; set; } = [];
}
