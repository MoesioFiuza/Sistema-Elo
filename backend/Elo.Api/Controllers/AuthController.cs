using Elo.Application.DTOs.Auth;
using Elo.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elo.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IAuthService authService,
    IValidator<LoginRequest> validator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return Ok(await authService.LoginAsync(request, ct));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioAtualDto>> Me(CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var id))
            return Unauthorized();

        return Ok(await authService.ObterAtualAsync(id, ct));
    }
}
