using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Pacientes;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Services;

public class PacienteService(IApplicationDbContext db) : IPacienteService
{
    public async Task<IReadOnlyList<PacienteDto>> BuscarAsync(string? termo, CancellationToken ct = default)
    {
        var query = db.Pacientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var t = termo.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(t) ||
                p.NumeroProntuario.ToLower().Contains(t));
        }

        return await query
            .OrderBy(p => p.Nome)
            .Take(50)
            .Select(p => new PacienteDto(
                p.Id,
                p.NumeroProntuario,
                p.Nome,
                p.DataNascimento,
                p.Sexo,
                p.HistoricoDiarreiaPrevia,
                p.HistoricoCdiff,
                p.CriadoEm))
            .ToListAsync(ct);
    }

    public async Task<PacienteDetalheDto> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var paciente = await db.Pacientes
            .AsNoTracking()
            .Include(p => p.Internacoes)
            .Include(p => p.Solicitacoes)
                .ThenInclude(s => s.ResultadoLaboratorial)
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException("Paciente não encontrado.");

        return MapDetalhe(paciente);
    }

    public async Task<PacienteDetalheDto> CriarAsync(CreatePacienteRequest request, CancellationToken ct = default)
    {
        var existe = await db.Pacientes.AnyAsync(
            p => p.NumeroProntuario == request.NumeroProntuario, ct);

        if (existe)
            throw new ConflictException("Já existe paciente com este prontuário.");

        var paciente = new Paciente
        {
            NumeroProntuario = request.NumeroProntuario.Trim(),
            Nome = request.Nome.Trim(),
            DataNascimento = request.DataNascimento,
            Sexo = request.Sexo,
            HistoricoDiarreiaPrevia = request.HistoricoDiarreiaPrevia,
            HistoricoCdiff = request.HistoricoCdiff,
            HistoricoCovid = request.HistoricoCovid,
        };

        var internacao = new Internacao
        {
            Enfermaria = request.Enfermaria.Trim(),
            Leito = request.Leito?.Trim(),
            DataInternacao = DateTime.UtcNow,
        };

        paciente.Internacoes.Add(internacao);
        db.Pacientes.Add(paciente);
        await db.SaveChangesAsync(ct);

        paciente = await db.Pacientes
            .AsNoTracking()
            .Include(p => p.Internacoes)
            .Include(p => p.Solicitacoes)
                .ThenInclude(s => s.ResultadoLaboratorial)
            .FirstAsync(p => p.Id == paciente.Id, ct);

        return MapDetalhe(paciente);
    }

    private static PacienteDetalheDto MapDetalhe(Paciente paciente) =>
        new(
            paciente.Id,
            paciente.NumeroProntuario,
            paciente.Nome,
            paciente.DataNascimento,
            paciente.Sexo,
            paciente.HistoricoDiarreiaPrevia,
            paciente.HistoricoCdiff,
            paciente.HistoricoCovid,
            paciente.HistoricoTransplante,
            paciente.HistoricoQuimioterapia,
            paciente.Internacoes
                .OrderByDescending(i => i.DataInternacao)
                .Select(i => new InternacaoResumoDto(
                    i.Id,
                    i.Enfermaria,
                    i.Leito,
                    i.DataInternacao,
                    i.DataAlta == null))
                .ToList(),
            paciente.Solicitacoes
                .OrderByDescending(s => s.CarimboDataHora)
                .Select(s => new ColetaHistoricoDto(
                    s.Id,
                    s.IdAmostraUnico,
                    s.Status,
                    s.CarimboDataHora,
                    s.DataColeta,
                    s.ResultadoLaboratorial?.DataResultado,
                    s.ResultadoLaboratorial?.TesteRapido,
                    s.ResultadoLaboratorial?.Cultura))
                .ToList());
}
