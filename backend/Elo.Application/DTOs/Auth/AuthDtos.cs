using Elo.Domain.Enums;

namespace Elo.Application.DTOs.Auth;

public record LoginRequest(string Email, string Senha);

public record LoginResponse(
    string Token,
    DateTime ExpiraEm,
    Guid UsuarioId,
    string Nome,
    string Email,
    PerfilUsuario Perfil);

public record UsuarioAtualDto(
    Guid Id,
    string Nome,
    string Email,
    PerfilUsuario Perfil,
    string? Setor = null);
