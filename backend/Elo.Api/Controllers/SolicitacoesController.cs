using Elo.Application.DTOs.Solicitacoes;
using Elo.Application.Services;
using Elo.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class SolicitacoesController(
    ISolicitacaoService solicitacaoService,
    IValidator<CreateSolicitacaoRequest> createValidator,
    IValidator<RegistrarResultadoRequest> resultadoValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> Listar(
        [FromQuery] StatusSolicitacao? status,
        CancellationToken ct)
    {
        var lista = await solicitacaoService.ListarAsync(status, ct);
        return Ok(lista);
    }

    [HttpGet("fila")]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> Fila(CancellationToken ct)
        => Ok(await solicitacaoService.ListarFilaLabAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> Obter(Guid id, CancellationToken ct)
    {
        var solicitacao = await solicitacaoService.ObterPorIdAsync(id, ct);
        return Ok(solicitacao);
    }

    [HttpPost]
    public async Task<ActionResult<SolicitacaoDetalheDto>> Criar(
        [FromBody] CreateSolicitacaoRequest request,
        CancellationToken ct)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        var solicitacao = await solicitacaoService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = solicitacao.Id }, solicitacao);
    }

    [HttpPost("{id:guid}/receber")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> ConfirmarRecebimento(Guid id, CancellationToken ct)
    {
        var solicitacao = await solicitacaoService.ConfirmarRecebimentoAsync(id, ct);
        return Ok(solicitacao);
    }

    [HttpPost("{id:guid}/resultado")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> RegistrarResultado(
        Guid id,
        [FromBody] RegistrarResultadoRequest request,
        CancellationToken ct)
    {
        await resultadoValidator.ValidateAndThrowAsync(request, ct);
        var solicitacao = await solicitacaoService.RegistrarResultadoAsync(id, request, ct);
        return Ok(solicitacao);
    }
}
