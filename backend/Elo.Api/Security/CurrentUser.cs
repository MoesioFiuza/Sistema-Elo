using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Elo.Application.Common.Interfaces;
using Elo.Domain.Enums;

namespace Elo.Api.Security;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UsuarioId
    {
        get
        {
            var raw = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public PerfilUsuario? Perfil
    {
        get
        {
            var raw = accessor.HttpContext?.User.FindFirstValue("perfil")
                ?? accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<PerfilUsuario>(raw, out var perfil) ? perfil : null;
        }
    }

    public string? Email =>
        accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? accessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email);

    public string? Nome => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public string? EnderecoIp => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public bool IsAdmin => Perfil == PerfilUsuario.Admin;

    public bool TemPerfil(params PerfilUsuario[] perfis) =>
        Perfil.HasValue && (IsAdmin || perfis.Contains(Perfil.Value));
}
