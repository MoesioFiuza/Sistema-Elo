using Elo.Application.DTOs.Tratamentos;
using Elo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class PesquisaController(ITratamentoService tratamentoService) : ControllerBase
{
    [HttpGet("tratamentos")]
    public async Task<ActionResult<IReadOnlyList<TratamentoDto>>> ListarTratamentos(CancellationToken ct)
        => Ok(await tratamentoService.ListarAsync(ct));

    [HttpPost("tratamentos")]
    public async Task<ActionResult<TratamentoDto>> UpsertTratamento(
        [FromBody] UpsertTratamentoRequest request,
        CancellationToken ct)
        => Ok(await tratamentoService.UpsertAsync(request, ct));

    [HttpPost("internacoes/{id:guid}/alta")]
    public async Task<IActionResult> Alta(Guid id, [FromBody] AltaInternacaoRequest request, CancellationToken ct)
    {
        await tratamentoService.AltaAsync(id, request, ct);
        return NoContent();
    }

    [HttpGet("pesquisa/cepa-desfecho")]
    public async Task<ActionResult<IReadOnlyList<CepaDesfechoDto>>> CepaDesfecho(CancellationToken ct)
        => Ok(await tratamentoService.CepaVsDesfechoAsync(ct));
}
