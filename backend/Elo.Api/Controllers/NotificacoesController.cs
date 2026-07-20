using Elo.Application.DTOs.Notificacoes;
using Elo.Application.Services;
using Elo.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/notificacoes")]
public class NotificacoesController(INotificacaoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificacaoDto>>> Listar(
        [FromQuery] bool naoLidas = false,
        CancellationToken ct = default)
    {
        var perfilClaim = User.FindFirstValue("perfil") ?? User.FindFirstValue(ClaimTypes.Role);
        Enum.TryParse<PerfilUsuario>(perfilClaim, out var perfil);
        Guid? usuarioId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : null;

        return Ok(await service.ListarAsync(usuarioId, perfil, naoLidas, ct));
    }

    [HttpPost("{id:guid}/lida")]
    public async Task<IActionResult> MarcarLida(Guid id, CancellationToken ct)
    {
        await service.MarcarLidaAsync(id, ct);
        return NoContent();
    }
}
