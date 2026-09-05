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
        [FromQuery] Guid? pacienteId,
        CancellationToken ct)
    {
        var lista = await solicitacaoService.ListarAsync(status, pacienteId, ct);
        return Ok(lista);
    }

    [HttpGet("fila")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> Fila(CancellationToken ct)
        => Ok(await solicitacaoService.ListarFilaLabAsync(ct));

    [HttpGet("historico")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> Historico(CancellationToken ct)
        => Ok(await solicitacaoService.ListarHistoricoLabAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> Obter(Guid id, CancellationToken ct)
    {
        var solicitacao = await solicitacaoService.ObterPorIdAsync(id, ct);
        return Ok(solicitacao);
    }

    [HttpPost]
    [Authorize(Policy = "Medico")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> Criar(
        [FromBody] CreateSolicitacaoRequest request,
        CancellationToken ct)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        var solicitacao = await solicitacaoService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = solicitacao.Id }, solicitacao);
    }

    [HttpPost("{id:guid}/receber")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> ConfirmarRecebimento(Guid id, CancellationToken ct)
    {
        var solicitacao = await solicitacaoService.RegistrarColetaAsync(id, ct);
        return Ok(solicitacao);
    }

    [HttpPost("{id:guid}/coleta")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> RegistrarColeta(Guid id, CancellationToken ct)
        => Ok(await solicitacaoService.RegistrarColetaAsync(id, ct));

    [HttpPost("{id:guid}/amostra")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> AvaliarAmostra(
        Guid id,
        [FromBody] AvaliarAmostraRequest request,
        CancellationToken ct)
        => Ok(await solicitacaoService.AvaliarAmostraAsync(id, request, ct));

    [HttpPost("{id:guid}/resultado")]
    [Authorize(Policy = "Laboratorio")]
    public async Task<ActionResult<SolicitacaoDetalheDto>> RegistrarResultado(
        Guid id,
        [FromBody] RegistrarResultadoRequest request,
        CancellationToken ct)
    {
        await resultadoValidator.ValidateAndThrowAsync(request, ct);
        var solicitacao = await solicitacaoService.RegistrarResultadoAsync(id, request, ct);
        return Ok(solicitacao);
    }

    [HttpGet("{id:guid}/laudo")]
    public async Task<ActionResult<LaudoDto>> Laudo(Guid id, CancellationToken ct)
        => Ok(await solicitacaoService.ObterLaudoAsync(id, ct));

    [HttpPost("{id:guid}/laudo-anexo")]
    [Authorize(Policy = "Laboratorio")]
    [RequestSizeLimit(8_388_608)]
    public async Task<ActionResult<SolicitacaoDetalheDto>> AnexarLaudo(
        Guid id,
        IFormFile arquivo,
        CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { erro = "Envie um arquivo de laudo." });

        await using var ms = new MemoryStream();
        await arquivo.CopyToAsync(ms, ct);
        var atualizada = await solicitacaoService.AnexarLaudoAsync(
            id,
            arquivo.FileName,
            arquivo.ContentType,
            ms.ToArray(),
            ct);
        return Ok(atualizada);
    }

    [HttpGet("{id:guid}/laudo-anexo")]
    public async Task<IActionResult> BaixarAnexo(Guid id, CancellationToken ct)
    {
        var (nome, tipo, bytes) = await solicitacaoService.BaixarAnexoAsync(id, ct);
        return File(bytes, tipo, nome);
    }
}
