using Elo.Domain.Enums;

namespace Elo.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? UsuarioId { get; }
    PerfilUsuario? Perfil { get; }
    string? Email { get; }
    string? Nome { get; }
    string? EnderecoIp { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    bool TemPerfil(params PerfilUsuario[] perfis);
}
