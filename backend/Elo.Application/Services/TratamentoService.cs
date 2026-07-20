using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Tratamentos;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Services;

public class TratamentoService(IApplicationDbContext db) : ITratamentoService
{
    public async Task<IReadOnlyList<TratamentoDto>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await db.TratamentosCdiff
            .AsNoTracking()
            .Include(t => t.SolicitacaoExame).ThenInclude(s => s.Paciente)
            .Include(t => t.SolicitacaoExame).ThenInclude(s => s.ResultadoLaboratorial)
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync(ct);

        return lista.Select(Map).ToList();
    }

    public async Task<TratamentoDto> UpsertAsync(UpsertTratamentoRequest request, CancellationToken ct = default)
    {
        var solicitacao = await db.SolicitacoesExame
            .Include(s => s.Paciente)
            .Include(s => s.ResultadoLaboratorial)
            .Include(s => s.TratamentoCdiff)
            .FirstOrDefaultAsync(s => s.Id == request.SolicitacaoExameId, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.Status != StatusSolicitacao.ResultadoLiberado)
            throw new ValidationAppException("Registre o resultado laboratorial antes do tratamento.");

        var t = solicitacao.TratamentoCdiff;
        if (t == null)
        {
            t = new TratamentoCdiff { SolicitacaoExameId = solicitacao.Id };
            db.TratamentosCdiff.Add(t);
            solicitacao.TratamentoCdiff = t;
        }

        t.IniciouTratamento = request.IniciouTratamento;
        t.DataInicioTratamento = request.DataInicioTratamento ?? DateTime.UtcNow;
        t.Medicacao = request.Medicacao?.Trim();
        t.Dose = request.Dose?.Trim();
        t.DuracaoDias = request.DuracaoDias;
        t.RespostaDia7 = request.RespostaDia7;
        t.RespostaFinal = request.RespostaFinal;
        t.Recidiva = request.Recidiva;
        t.DataRecidiva = request.DataRecidiva;
        t.ObservacoesTratamento = request.ObservacoesTratamento;

        await db.SaveChangesAsync(ct);

        var salvo = await db.TratamentosCdiff
            .AsNoTracking()
            .Include(x => x.SolicitacaoExame).ThenInclude(s => s.Paciente)
            .Include(x => x.SolicitacaoExame).ThenInclude(s => s.ResultadoLaboratorial)
            .FirstAsync(x => x.Id == t.Id, ct);

        return Map(salvo);
    }

    public async Task AltaAsync(Guid internacaoId, AltaInternacaoRequest request, CancellationToken ct = default)
    {
        var internacao = await db.Internacoes.FirstOrDefaultAsync(i => i.Id == internacaoId, ct)
            ?? throw new NotFoundException("Internação não encontrada.");

        if (internacao.DataAlta != null)
            throw new ConflictException("Internação já possui alta.");

        internacao.DataAlta = request.DataAlta ?? DateTime.UtcNow;
        internacao.IsolamentoAtivo = false;

        if (request.Obito)
        {
            internacao.Obito = SimNaoNaoRegistrado.Sim;
            internacao.DataObito = internacao.DataAlta;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CepaDesfechoDto>> CepaVsDesfechoAsync(CancellationToken ct = default)
    {
        var dados = await db.ResultadosLaboratoriais
            .AsNoTracking()
            .Include(r => r.SolicitacaoExame).ThenInclude(s => s.TratamentoCdiff)
            .Include(r => r.SolicitacaoExame).ThenInclude(s => s.Internacao)
            .Where(r => r.CepaIdentificada != null && r.CepaIdentificada != "")
            .ToListAsync(ct);

        return dados
            .GroupBy(r => r.CepaIdentificada!.Trim())
            .Select(g => new CepaDesfechoDto(
                g.Key,
                g.Count(),
                g.Count(r => r.SolicitacaoExame.TratamentoCdiff?.RespostaFinal is RespostaClinica.Melhora or RespostaClinica.Cura),
                g.Count(r => r.SolicitacaoExame.TratamentoCdiff?.Recidiva == SimNaoNaoRegistrado.Sim),
                g.Count(r => r.SolicitacaoExame.Internacao.Obito == SimNaoNaoRegistrado.Sim)))
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    private static TratamentoDto Map(TratamentoCdiff t) =>
        new(
            t.Id,
            t.SolicitacaoExameId,
            t.SolicitacaoExame.IdAmostraUnico,
            t.SolicitacaoExame.Paciente.Nome,
            t.IniciouTratamento,
            t.DataInicioTratamento,
            t.Medicacao,
            t.Dose,
            t.DuracaoDias,
            t.RespostaDia7,
            t.RespostaFinal,
            t.Recidiva,
            t.DataRecidiva,
            t.ObservacoesTratamento,
            t.SolicitacaoExame.ResultadoLaboratorial?.CepaIdentificada);
}
