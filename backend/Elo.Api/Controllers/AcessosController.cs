using Elo.Application.DTOs.Auth;
using Elo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/v1/acessos")]
public class AcessosController(IAcessoService acessoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoAcessoDto>>> Listar(CancellationToken ct)
        => Ok(await acessoService.ListarAsync(ct));

    [HttpPost("{id:guid}/aprovar")]
    public async Task<ActionResult<AprovarAcessoResponse>> Aprovar(
        Guid id,
        [FromBody] AprovarAcessoRequest? request,
        CancellationToken ct)
        => Ok(await acessoService.AprovarAsync(id, request ?? new AprovarAcessoRequest(), ct));

    [HttpPost("{id:guid}/recusar")]
    public async Task<IActionResult> Recusar(
        Guid id,
        [FromBody] RecusarAcessoRequest request,
        CancellationToken ct)
    {
        await acessoService.RecusarAsync(id, request, ct);
        return NoContent();
    }
}
