using Elo.Application.DTOs.Auth;

namespace Elo.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UsuarioAtualDto> ObterAtualAsync(Guid usuarioId, CancellationToken ct = default);
}
