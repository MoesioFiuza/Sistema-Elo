using Elo.Application.DTOs.Dashboard;
using Elo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize(Policy = "Vigilancia")]
[Route("api/v1/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("resumo")]
    public async Task<ActionResult<DashboardResumoDto>> Resumo(CancellationToken ct)
    {
        var resumo = await dashboardService.ObterResumoAsync(ct);
        return Ok(resumo);
    }
}
