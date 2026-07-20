using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Auth;
using Elo.Application.Options;
using Elo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Elo.Application.Services;

public class AuthService(
    IApplicationDbContext db,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.Ativo, ct)
            ?? throw new ValidationAppException("E-mail ou senha inválidos.");

        var result = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha);
        if (result == PasswordVerificationResult.Failed)
            throw new ValidationAppException("E-mail ou senha inválidos.");

        var expira = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);
        var token = GerarToken(usuario, expira);

        return new LoginResponse(
            token,
            expira,
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil);
    }

    public async Task<UsuarioAtualDto> ObterAtualAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await db.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Ativo, ct)
            ?? throw new NotFoundException("Usuário não encontrado.");

        return new UsuarioAtualDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
    }

    private string GerarToken(Usuario usuario, DateTime expira)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim("perfil", usuario.Perfil.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
