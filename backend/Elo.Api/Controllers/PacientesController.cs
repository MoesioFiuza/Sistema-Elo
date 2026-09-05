using Elo.Application.DTOs.Pacientes;
using Elo.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class PacientesController(
    IPacienteService pacienteService,
    IValidator<CreatePacienteRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PacienteDto>>> Buscar(
        [FromQuery] string? q,
        CancellationToken ct)
    {
        var pacientes = await pacienteService.BuscarAsync(q, ct);
        return Ok(pacientes);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PacienteDetalheDto>> Obter(Guid id, CancellationToken ct)
    {
        var paciente = await pacienteService.ObterPorIdAsync(id, ct);
        return Ok(paciente);
    }

    [HttpPost]
    [Authorize(Policy = "Medico")]
    public async Task<ActionResult<PacienteDetalheDto>> Criar(
        [FromBody] CreatePacienteRequest request,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        var paciente = await pacienteService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = paciente.Id }, paciente);
    }
}
