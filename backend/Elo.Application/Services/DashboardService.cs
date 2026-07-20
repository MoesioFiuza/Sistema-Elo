using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Dashboard;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Services;

public class DashboardService(IApplicationDbContext db) : IDashboardService
{
    public async Task<DashboardResumoDto> ObterResumoAsync(CancellationToken ct = default)
    {
        var pendentes = await db.SolicitacoesExame.CountAsync(s => s.Status == StatusSolicitacao.Pendente, ct);
        var emAnalise = await db.SolicitacoesExame.CountAsync(s => s.Status == StatusSolicitacao.EmAnalise, ct);

        var positivos = await db.ResultadosLaboratoriais
            .CountAsync(r => r.TesteRapido == ResultadoTeste.Positivo || r.AlertaPositivoEnviado, ct);
        var negativos = await db.ResultadosLaboratoriais
            .CountAsync(r => r.TesteRapido == ResultadoTeste.Negativo, ct);

        var isolamento = await db.Internacoes.CountAsync(i => i.IsolamentoAtivo && i.DataAlta == null, ct);

        // Materializa e agrega em memória — GroupBy com navegação aninhada não traduz no EF/Npgsql
        var linhas = await db.SolicitacoesExame
            .AsNoTracking()
            .Select(s => new
            {
                s.Internacao.Enfermaria,
                Positivo = s.ResultadoLaboratorial != null &&
                    (s.ResultadoLaboratorial.TesteRapido == ResultadoTeste.Positivo ||
                     s.ResultadoLaboratorial.AlertaPositivoEnviado)
            })
            .ToListAsync(ct);

        var porEnfermaria = linhas
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Enfermaria) ? "Sem enfermaria" : x.Enfermaria)
            .Select(g => new EnfermariaResumoDto(
                g.Key,
                g.Count(),
                g.Count(x => x.Positivo)))
            .OrderByDescending(e => e.Positivos)
            .ThenByDescending(e => e.Total)
            .ToList();

        var alertas = await db.SolicitacoesExame
            .AsNoTracking()
            .Where(s => s.ResultadoLaboratorial != null && s.ResultadoLaboratorial.AlertaPositivoEnviado)
            .OrderByDescending(s => s.ResultadoLaboratorial!.DataResultado)
            .Take(10)
            .Select(s => new AlertaRecenteDto(
                s.Id,
                s.IdAmostraUnico,
                s.Paciente.Nome,
                s.Internacao.Enfermaria,
                s.ResultadoLaboratorial!.DataResultado,
                s.Internacao.IsolamentoAtivo,
                s.Internacao.Leito))
            .ToListAsync(ct);

        return new DashboardResumoDto(
            pendentes,
            emAnalise,
            positivos,
            negativos,
            isolamento,
            porEnfermaria,
            alertas);
    }
}
