using Elo.Application.DTOs.Auth;
using Elo.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Elo.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IAuthService authService,
    IAcessoService acessoService,
    IValidator<LoginRequest> loginValidator,
    IValidator<SolicitarAcessoRequest> acessoValidator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        await loginValidator.ValidateAndThrowAsync(request, ct);
        return Ok(await authService.LoginAsync(request, ct));
    }

    [HttpPost("solicitar-acesso")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<SolicitarAcessoResponse>> SolicitarAcesso(
        [FromBody] SolicitarAcessoRequest request,
        CancellationToken ct)
    {
        await acessoValidator.ValidateAndThrowAsync(request, ct);
        return Ok(await acessoService.SolicitarAsync(request, ct));
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
