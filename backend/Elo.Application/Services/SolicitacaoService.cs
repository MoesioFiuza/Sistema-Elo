using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Solicitacoes;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elo.Application.Services;

public class SolicitacaoService(IApplicationDbContext db) : ISolicitacaoService
{
    public async Task<IReadOnlyList<SolicitacaoDto>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default)
    {
        var query = db.SolicitacoesExame
            .AsNoTracking()
            .Include(s => s.Paciente)
            .Include(s => s.Internacao)
            .Include(s => s.ResultadoLaboratorial)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query
            .OrderByDescending(s => s.CarimboDataHora)
            .Select(s => new SolicitacaoDto(
                s.Id,
                s.IdAmostraUnico,
                s.Status,
                s.CarimboDataHora,
                s.Paciente.Nome,
                s.Paciente.NumeroProntuario,
                s.Internacao.Enfermaria,
                s.Internacao.Leito,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.TesteRapido : null))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SolicitacaoDto>> ListarFilaLabAsync(CancellationToken ct = default)
    {
        return await db.SolicitacoesExame
            .AsNoTracking()
            .Where(s => s.Status == StatusSolicitacao.Pendente || s.Status == StatusSolicitacao.EmAnalise)
            .OrderByDescending(s => s.CarimboDataHora)
            .Select(s => new SolicitacaoDto(
                s.Id,
                s.IdAmostraUnico,
                s.Status,
                s.CarimboDataHora,
                s.Paciente.Nome,
                s.Paciente.NumeroProntuario,
                s.Internacao.Enfermaria,
                s.Internacao.Leito,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.TesteRapido : null))
            .ToListAsync(ct);
    }

    public async Task<SolicitacaoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var solicitacao = await CarregarDetalheQuery()
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        return MapDetalhe(solicitacao);
    }

    public async Task<SolicitacaoDetalheDto> CriarAsync(CreateSolicitacaoRequest request, CancellationToken ct = default)
    {
        var form = request.Formulario;
        if (form.Diarreia != SimNaoNaoRegistrado.Sim)
            throw new ValidationAppException("Filtro clínico: solicite exame apenas com diarreia confirmada.");

        var paciente = await db.Pacientes
            .Include(p => p.Internacoes)
            .FirstOrDefaultAsync(p => p.Id == request.PacienteId, ct)
            ?? throw new NotFoundException("Paciente não encontrado.");

        var internacao = paciente.Internacoes.FirstOrDefault(i => i.Id == request.InternacaoId)
            ?? throw new NotFoundException("Internação não encontrada.");

        if (internacao.DataAlta != null)
            throw new ValidationAppException("Paciente não está internado.");

        if (request.HistoricoPaciente is { } h)
        {
            paciente.DiarreiaAssociadaAtbPassado = h.DiarreiaAssociadaAtbPassado;
            paciente.ProcurouAtendimentoPorDiarreia = h.ProcurouAtendimentoPorDiarreia;
            paciente.InternadoPorDiarreia = h.InternadoPorDiarreia;
            paciente.QuandoInternadoPorDiarreia = h.QuandoInternadoPorDiarreia;
            paciente.HistoricoCdiff = h.HistoricoCdiff;
            paciente.HistoricoDiarreiaPrevia = h.DiarreiaAssociadaAtbPassado;
            paciente.CdiffFamiliaAmbiente = h.CdiffFamiliaAmbiente;
            paciente.ProblemasSaudeAdjacentes = h.ProblemasSaudeAdjacentes;
            paciente.ProblemasSaudeOutros = h.ProblemasSaudeOutros;
            paciente.HistoricoCovid = h.HistoricoCovid;
            paciente.CovidAnosPositivos = h.CovidAnosPositivos;
            paciente.CovidTeveSintomas = h.CovidTeveSintomas;
            paciente.CovidSintomasDescricao = h.CovidSintomasDescricao;
            paciente.CovidInternado = h.CovidInternado;
            paciente.CovidDiasInternacao = h.CovidDiasInternacao;
            paciente.CovidOxigenioOuTratamentos = h.CovidOxigenioOuTratamentos;
            paciente.CovidTratamentosDescricao = h.CovidTratamentosDescricao;
            paciente.CovidIntubado = h.CovidIntubado;
            paciente.CovidQuandoIntubacao = h.CovidQuandoIntubacao;
            paciente.CovidDiasIntubado = h.CovidDiasIntubado;
            paciente.CovidUtiDuranteIntubacao = h.CovidUtiDuranteIntubacao;
        }

        if (request.Internacao is { } fi)
        {
            internacao.MotivoInternacao = fi.MotivoInternacao;
            internacao.TipoCirurgia = fi.TipoCirurgia;
            internacao.ParaTcth = fi.ParaTcth;
            internacao.ParaTos = fi.ParaTos;
            internacao.InternouComDiarreia = fi.InternouComDiarreia;
            internacao.UsoImunossupressoresDurante = fi.UsoImunossupressoresDurante;
            internacao.UsoImunossupressoresAtual = fi.UsoImunossupressoresAtual;
            internacao.ImunossupressoresDescricao = fi.ImunossupressoresDescricao;
            internacao.EmUti = fi.EmUti;
            internacao.Leucocitose = fi.Leucocitose;
            internacao.Leucopenia = fi.Leucopenia;
            internacao.Sepse = fi.Sepse;
            internacao.Obito = fi.Obito;
            if (fi.Obito == SimNaoNaoRegistrado.Sim)
                internacao.DataObito ??= DateTime.UtcNow;
        }

        var solicitante = await db.Usuarios
            .Where(u => u.Ativo && u.Perfil == PerfilUsuario.Medico)
            .OrderBy(u => u.CriadoEm)
            .FirstOrDefaultAsync(ct)
            ?? throw new ValidationAppException("Nenhum médico cadastrado no sistema.");

        var solicitacao = new SolicitacaoExame
        {
            PacienteId = paciente.Id,
            InternacaoId = internacao.Id,
            SolicitanteId = solicitante.Id,
            CarimboDataHora = DateTime.UtcNow,
            IdAmostraUnico = await GerarIdAmostraAsync(ct),
            Status = StatusSolicitacao.Pendente,
            FormularioClinico = new FormularioClinico
            {
                Diarreia = form.Diarreia,
                DiasInicioSintomas = form.DiasInicioSintomas,
                EpisodiosDiarreia24h = form.EpisodiosDiarreia24h,
                ConsistenciaFezes = form.ConsistenciaFezes,
                SintomasAssociados = form.SintomasAssociados,
                UsoIbpAntesDiarreia = form.UsoIbpAntesDiarreia,
                UsoIbpDuranteDiarreia = form.UsoIbpDuranteDiarreia,
                UsoIbp = form.UsoIbpDuranteDiarreia != SimNaoNaoRegistrado.NaoRegistrado
                    ? form.UsoIbpDuranteDiarreia
                    : form.UsoIbpAntesDiarreia,
                IbpDescricao = form.IbpDescricao,
                DorAbdominal = form.DorAbdominal,
                Febre = form.Febre,
                TemperaturaMaxima = form.TemperaturaMaxima,
                DuracaoFebre = form.DuracaoFebre,
                Peritonite = form.Peritonite,
                VentilacaoMecanica = form.VentilacaoMecanica,
                InternouUtiDurante = form.InternouUtiDurante,
                Leucocitose = form.Leucocitose,
                Leucopenia = form.Leucopenia,
                FezIra = form.FezIra,
                DrogasVasoativas = form.DrogasVasoativas,
                DesorientacaoConfusao = form.DesorientacaoConfusao,
                UsoAntimicrobianoAntesColeta = form.UsoAntimicrobianoAntesColeta,
                AntimicrobianosAntesDescricao = form.AntimicrobianosAntesDescricao,
                UsoAntimicrobianoDiaColeta = form.UsoAntimicrobianoDiaColeta,
                AntimicrobianosDiaColetaDescricao = form.AntimicrobianosDiaColetaDescricao,
                UsoAntimicrobiano30d = form.UsoAntimicrobianoAntesColeta,
                AntimicrobianosDescricao = form.AntimicrobianosAntesDescricao
                    ?? form.AntimicrobianosDiaColetaDescricao,
                ObservacoesClinicas = form.ObservacoesClinicas,
            },
        };

        db.SolicitacoesExame.Add(solicitacao);
        await db.SaveChangesAsync(ct);

        var criada = await CarregarDetalheQuery()
            .FirstAsync(s => s.Id == solicitacao.Id, ct);

        return MapDetalhe(criada);
    }

    public async Task<SolicitacaoDetalheDto> ConfirmarRecebimentoAsync(Guid id, CancellationToken ct = default)
    {
        var solicitacao = await db.SolicitacoesExame
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.Status != StatusSolicitacao.Pendente)
            throw new ValidationAppException("Somente solicitações pendentes podem ser recebidas.");

        solicitacao.Status = StatusSolicitacao.EmAnalise;
        solicitacao.DataRecebimentoLaboratorio = DateTime.UtcNow;
        solicitacao.DataColeta ??= DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var atualizada = await CarregarDetalheQuery()
            .FirstAsync(s => s.Id == id, ct);

        return MapDetalhe(atualizada);
    }

    public async Task<SolicitacaoDetalheDto> RegistrarResultadoAsync(
        Guid id,
        RegistrarResultadoRequest request,
        CancellationToken ct = default)
    {
        var solicitacao = await db.SolicitacoesExame
            .Include(s => s.ResultadoLaboratorial)
            .Include(s => s.Paciente)
            .Include(s => s.Internacao)
            .Include(s => s.Solicitante)
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.Status is StatusSolicitacao.Pendente or StatusSolicitacao.Cancelado)
            throw new ValidationAppException("Confirme o recebimento antes de lançar o resultado.");

        if (solicitacao.ResultadoLaboratorial != null)
            throw new ConflictException("Resultado já registrado para esta solicitação.");

        var responsavel = await db.Usuarios
            .Where(u => u.Ativo && u.Perfil == PerfilUsuario.Laboratorio)
            .OrderBy(u => u.CriadoEm)
            .FirstOrDefaultAsync(ct);

        var positivo = request.TesteRapido == ResultadoTeste.Positivo
            || request.ToxinaA == ResultadoTeste.Positivo
            || request.ToxinaB == ResultadoTeste.Positivo
            || request.Cultura == ResultadoTeste.Positivo;

        solicitacao.Status = StatusSolicitacao.ResultadoLiberado;
        solicitacao.ResultadoLaboratorial = new ResultadoLaboratorial
        {
            ResponsavelId = responsavel?.Id,
            DataResultado = DateTime.UtcNow,
            TesteRapido = request.TesteRapido,
            ToxinaA = request.ToxinaA,
            ToxinaB = request.ToxinaB,
            Cultura = request.Cultura,
            CepaIdentificada = request.CepaIdentificada,
            ObservacoesLaboratorio = request.ObservacoesLaboratorio,
            AlertaPositivoEnviado = positivo,
            DataAlertaEnviado = positivo ? DateTime.UtcNow : null,
            LiberacaoIsolamentoEnviada = !positivo && solicitacao.Internacao.IsolamentoAtivo,
            DataLiberacaoIsolamento = !positivo && solicitacao.Internacao.IsolamentoAtivo
                ? DateTime.UtcNow
                : null,
        };

        if (positivo)
        {
            solicitacao.Internacao.IsolamentoAtivo = true;
            await CriarAlertasPositivoAsync(solicitacao, ct);
        }
        else
        {
            var tinhaIsolamento = solicitacao.Internacao.IsolamentoAtivo;
            solicitacao.Internacao.IsolamentoAtivo = false;
            if (tinhaIsolamento || request.TesteRapido == ResultadoTeste.Negativo)
                await CriarAlertasNegativoAsync(solicitacao, ct);
        }

        await db.SaveChangesAsync(ct);

        var atualizada = await CarregarDetalheQuery()
            .FirstAsync(s => s.Id == id, ct);

        return MapDetalhe(atualizada);
    }

    private async Task CriarAlertasPositivoAsync(SolicitacaoExame s, CancellationToken ct)
    {
        var titulo = "ISOLAR PACIENTE";
        var msg = $"{s.Paciente.Nome} · {s.Internacao.Enfermaria} · Amostra {s.IdAmostraUnico} — resultado positivo. Isolamento imediato.";

        foreach (var perfil in new[] { PerfilUsuario.CCIH, PerfilUsuario.Enfermagem, PerfilUsuario.Medico })
        {
            db.Notificacoes.Add(new Notificacao
            {
                Id = Guid.NewGuid(),
                CriadoEm = DateTime.UtcNow,
                PerfilDestino = perfil,
                UsuarioDestinoId = perfil == PerfilUsuario.Medico ? s.SolicitanteId : null,
                Tipo = TipoNotificacao.Isolamento,
                Titulo = titulo,
                Mensagem = msg,
                SolicitacaoExameId = s.Id,
            });
        }

        await Task.CompletedTask;
    }

    private async Task CriarAlertasNegativoAsync(SolicitacaoExame s, CancellationToken ct)
    {
        var msg = $"{s.Paciente.Nome} · {s.Internacao.Enfermaria} · Amostra {s.IdAmostraUnico} — resultado negativo. Liberar isolamento / fechar suspeita.";

        foreach (var perfil in new[] { PerfilUsuario.CCIH, PerfilUsuario.Enfermagem, PerfilUsuario.Medico })
        {
            db.Notificacoes.Add(new Notificacao
            {
                Id = Guid.NewGuid(),
                CriadoEm = DateTime.UtcNow,
                PerfilDestino = perfil,
                UsuarioDestinoId = perfil == PerfilUsuario.Medico ? s.SolicitanteId : null,
                Tipo = TipoNotificacao.Liberacao,
                Titulo = "Liberar isolamento",
                Mensagem = msg,
                SolicitacaoExameId = s.Id,
            });
        }

        await Task.CompletedTask;
    }

    private IQueryable<SolicitacaoExame> CarregarDetalheQuery() =>
        db.SolicitacoesExame
            .AsNoTracking()
            .Include(s => s.Paciente)
            .Include(s => s.Internacao)
            .Include(s => s.FormularioClinico)
            .Include(s => s.ResultadoLaboratorial);

    private static SolicitacaoDetalheDto MapDetalhe(SolicitacaoExame s) =>
        new(
            s.Id,
            s.IdAmostraUnico,
            s.Status,
            s.CarimboDataHora,
            s.DataColeta,
            s.DataRecebimentoLaboratorio,
            s.Paciente.Nome,
            s.Paciente.NumeroProntuario,
            s.Internacao.Enfermaria,
            s.FormularioClinico == null ? null : new FormularioClinicoDto(
                s.FormularioClinico.Diarreia,
                s.FormularioClinico.DiasInicioSintomas,
                s.FormularioClinico.EpisodiosDiarreia24h,
                s.FormularioClinico.ConsistenciaFezes,
                s.FormularioClinico.SintomasAssociados,
                s.FormularioClinico.UsoIbpAntesDiarreia,
                s.FormularioClinico.UsoIbpDuranteDiarreia,
                s.FormularioClinico.DorAbdominal,
                s.FormularioClinico.Febre,
                s.FormularioClinico.TemperaturaMaxima,
                s.FormularioClinico.DuracaoFebre,
                s.FormularioClinico.Peritonite,
                s.FormularioClinico.VentilacaoMecanica,
                s.FormularioClinico.InternouUtiDurante,
                s.FormularioClinico.Leucocitose,
                s.FormularioClinico.Leucopenia,
                s.FormularioClinico.FezIra,
                s.FormularioClinico.DrogasVasoativas,
                s.FormularioClinico.DesorientacaoConfusao,
                s.FormularioClinico.UsoAntimicrobianoAntesColeta,
                s.FormularioClinico.AntimicrobianosAntesDescricao,
                s.FormularioClinico.UsoAntimicrobianoDiaColeta,
                s.FormularioClinico.AntimicrobianosDiaColetaDescricao,
                s.FormularioClinico.ObservacoesClinicas),
            s.ResultadoLaboratorial == null ? null : new ResultadoLaboratorialDto(
                s.ResultadoLaboratorial.DataResultado,
                s.ResultadoLaboratorial.TesteRapido,
                s.ResultadoLaboratorial.ToxinaA,
                s.ResultadoLaboratorial.ToxinaB,
                s.ResultadoLaboratorial.Cultura,
                s.ResultadoLaboratorial.CepaIdentificada,
                s.ResultadoLaboratorial.AlertaPositivoEnviado));

    private async Task<string> GerarIdAmostraAsync(CancellationToken ct)
    {
        var hoje = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefixo = $"ELO-{hoje}-";

        var ultimo = await db.SolicitacoesExame
            .Where(s => s.IdAmostraUnico.StartsWith(prefixo))
            .OrderByDescending(s => s.IdAmostraUnico)
            .Select(s => s.IdAmostraUnico)
            .FirstOrDefaultAsync(ct);

        var sequencia = 1;
        if (ultimo != null)
        {
            var parte = ultimo[prefixo.Length..];
            if (int.TryParse(parte, out var n))
                sequencia = n + 1;
        }

        return $"{prefixo}{sequencia:D4}";
    }
}
