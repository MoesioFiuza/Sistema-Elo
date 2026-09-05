using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            sistema = "Cdigital",
            laboratorio = "NEPEC",
            versao = "1.0.0",
            timestamp = DateTime.UtcNow
        });
    }
}
