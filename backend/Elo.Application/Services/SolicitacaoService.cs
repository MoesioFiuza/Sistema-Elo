using Elo.Application.Common;
using Elo.Application.Common.Interfaces;
using Elo.Application.DTOs.Solicitacoes;
using Elo.Application.Options;
using Elo.Domain.Entities;
using Elo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Elo.Application.Services;

public class SolicitacaoService(
    IApplicationDbContext db,
    ICurrentUser currentUser,
    IAuditoriaService auditoria,
    IOptions<PlataformaOptions> plataformaOptions) : ISolicitacaoService
{
    private readonly PlataformaOptions _plataforma = plataformaOptions.Value;

    public async Task<IReadOnlyList<SolicitacaoDto>> ListarAsync(
        StatusSolicitacao? status,
        Guid? pacienteId,
        CancellationToken ct = default)
    {
        var query = db.SolicitacoesExame.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (pacienteId.HasValue)
            query = query.Where(s => s.PacienteId == pacienteId.Value);

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
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.TesteRapido : null,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.Cultura : null,
                s.QualidadeAmostra))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SolicitacaoDto>> ListarFilaLabAsync(CancellationToken ct = default)
    {
        return await db.SolicitacoesExame
            .AsNoTracking()
            .Where(s =>
                s.Status == StatusSolicitacao.Pendente ||
                s.Status == StatusSolicitacao.Coletado ||
                s.Status == StatusSolicitacao.EmAnalise ||
                s.Status == StatusSolicitacao.AmostraInsatisfatoria)
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
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.TesteRapido : null,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.Cultura : null,
                s.QualidadeAmostra))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SolicitacaoDto>> ListarHistoricoLabAsync(CancellationToken ct = default)
    {
        return await db.SolicitacoesExame
            .AsNoTracking()
            .Where(s => s.Status == StatusSolicitacao.ResultadoLiberado)
            .OrderByDescending(s => s.CarimboDataHora)
            .Take(80)
            .Select(s => new SolicitacaoDto(
                s.Id,
                s.IdAmostraUnico,
                s.Status,
                s.CarimboDataHora,
                s.Paciente.Nome,
                s.Paciente.NumeroProntuario,
                s.Internacao.Enfermaria,
                s.Internacao.Leito,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.TesteRapido : null,
                s.ResultadoLaboratorial != null ? s.ResultadoLaboratorial.Cultura : null,
                s.QualidadeAmostra))
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

        if (form.EpisodiosDiarreia24h is null or < 3)
            throw new ValidationAppException("Diarreia: informe pelo menos 3 episódios em 24 horas.");

        if (form.ConsistenciaFezes is not (ConsistenciaFezes.Liquida or ConsistenciaFezes.Pastosa))
            throw new ValidationAppException("Diarreia: a consistência deve ser líquida ou pastosa.");

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

        var solicitanteId = currentUser.UsuarioId
            ?? throw new ForbiddenException("Sessão inválida.");

        var solicitacao = new SolicitacaoExame
        {
            Id = Guid.NewGuid(),
            PacienteId = paciente.Id,
            InternacaoId = internacao.Id,
            SolicitanteId = solicitanteId,
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
        auditoria.Registrar("SolicitacaoExame", solicitacao.Id, "criar", solicitacao.IdAmostraUnico);
        await db.SaveChangesAsync(ct);

        var criada = await CarregarDetalheQuery()
            .FirstAsync(s => s.Id == solicitacao.Id, ct);

        return MapDetalhe(criada);
    }

    public async Task<SolicitacaoDetalheDto> RegistrarColetaAsync(Guid id, CancellationToken ct = default)
    {
        var solicitacao = await db.SolicitacoesExame
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.Status is not (StatusSolicitacao.Pendente or StatusSolicitacao.AmostraInsatisfatoria))
            throw new ValidationAppException("Só é possível registrar coleta em solicitação em andamento ou amostra insatisfatória.");

        solicitacao.Status = StatusSolicitacao.Coletado;
        solicitacao.DataColeta = DateTime.UtcNow;
        solicitacao.DataRecebimentoLaboratorio = DateTime.UtcNow;
        solicitacao.QualidadeAmostra = QualidadeAmostra.NaoAvaliada;
        solicitacao.DataAvaliacaoAmostra = null;

        auditoria.Registrar("SolicitacaoExame", solicitacao.Id, "coleta", solicitacao.IdAmostraUnico);
        await db.SaveChangesAsync(ct);
        return await RecarregarDetalheAsync(id, ct);
    }

    public async Task<SolicitacaoDetalheDto> AvaliarAmostraAsync(
        Guid id,
        AvaliarAmostraRequest request,
        CancellationToken ct = default)
    {
        var solicitacao = await db.SolicitacoesExame
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.Status is not (StatusSolicitacao.Coletado or StatusSolicitacao.EmAnalise or StatusSolicitacao.AmostraInsatisfatoria))
            throw new ValidationAppException("Avalie a amostra após registrar a coleta.");

        if (request.Qualidade is not (QualidadeAmostra.Satisfatoria or QualidadeAmostra.Insatisfatoria))
            throw new ValidationAppException("Informe se a amostra é satisfatória ou insatisfatória.");

        solicitacao.QualidadeAmostra = request.Qualidade;
        solicitacao.DataAvaliacaoAmostra = DateTime.UtcNow;
        solicitacao.Status = request.Qualidade == QualidadeAmostra.Satisfatoria
            ? StatusSolicitacao.EmAnalise
            : StatusSolicitacao.AmostraInsatisfatoria;

        auditoria.Registrar("SolicitacaoExame", solicitacao.Id, "avaliar-amostra", request.Qualidade.ToString());
        await db.SaveChangesAsync(ct);
        return await RecarregarDetalheAsync(id, ct);
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

        if (solicitacao.Status != StatusSolicitacao.EmAnalise)
            throw new ValidationAppException("A amostra precisa estar satisfatória (testagem em andamento) para lançar o resultado.");

        if (solicitacao.ResultadoLaboratorial != null)
            throw new ConflictException("Resultado já registrado para esta solicitação. Abra uma nova coleta para o mesmo paciente.");

        if (string.IsNullOrWhiteSpace(request.AssinaturaBase64))
            throw new ValidationAppException("A assinatura do responsável é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.AssinadoPorNome) && string.IsNullOrWhiteSpace(currentUser.Nome))
            throw new ValidationAppException("Informe o nome de quem assina o laudo.");

        var positivo = request.TesteRapido == ResultadoTeste.Positivo
            || request.Cultura == ResultadoTeste.Positivo
            || request.ToxinaA == ResultadoTeste.Positivo
            || request.ToxinaB == ResultadoTeste.Positivo;

        solicitacao.Status = StatusSolicitacao.ResultadoLiberado;
        solicitacao.ResultadoLaboratorial = new ResultadoLaboratorial
        {
            ResponsavelId = currentUser.UsuarioId,
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
            AssinaturaBase64 = NormalizarAssinatura(request.AssinaturaBase64),
            AssinadoPorNome = string.IsNullOrWhiteSpace(request.AssinadoPorNome)
                ? currentUser.Nome
                : request.AssinadoPorNome.Trim(),
            AssinadoEm = DateTime.UtcNow,
            LaudoGeradoEm = DateTime.UtcNow,
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

        auditoria.Registrar(
            "SolicitacaoExame",
            solicitacao.Id,
            "resultado",
            $"{solicitacao.IdAmostraUnico} TR={request.TesteRapido} Cultura={request.Cultura}");
        await db.SaveChangesAsync(ct);
        return await RecarregarDetalheAsync(id, ct);
    }

    public async Task<SolicitacaoDetalheDto> AnexarLaudoAsync(
        Guid id,
        string nomeArquivo,
        string contentType,
        byte[] bytes,
        CancellationToken ct = default)
    {
        if (bytes.Length == 0)
            throw new ValidationAppException("Arquivo vazio.");

        if (bytes.Length > 8 * 1024 * 1024)
            throw new ValidationAppException("O anexo deve ter no máximo 8 MB.");

        var permitido = contentType is "application/pdf" or "image/png" or "image/jpeg";
        if (!permitido)
            throw new ValidationAppException("Anexe um PDF, PNG ou JPG.");

        var solicitacao = await db.SolicitacoesExame
            .Include(s => s.ResultadoLaboratorial)
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (solicitacao.ResultadoLaboratorial is null)
            throw new ValidationAppException("Lance o resultado antes de anexar o laudo.");

        solicitacao.ResultadoLaboratorial.LaudoAnexoNome = Path.GetFileName(nomeArquivo);
        solicitacao.ResultadoLaboratorial.LaudoAnexoContentType = contentType;
        solicitacao.ResultadoLaboratorial.LaudoAnexoBytes = bytes;
        solicitacao.ResultadoLaboratorial.LaudoGeradoEm ??= DateTime.UtcNow;

        auditoria.Registrar("SolicitacaoExame", solicitacao.Id, "anexar-laudo", nomeArquivo);
        await db.SaveChangesAsync(ct);
        return await RecarregarDetalheAsync(id, ct);
    }

    public async Task<LaudoDto> ObterLaudoAsync(Guid id, CancellationToken ct = default)
    {
        var s = await CarregarDetalheQuery()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Solicitação não encontrada.");

        if (s.ResultadoLaboratorial is null)
            throw new ValidationAppException("Ainda não há resultado para gerar o laudo.");

        var r = s.ResultadoLaboratorial;
        return new LaudoDto(
            s.Id,
            s.IdAmostraUnico,
            s.Paciente.Nome,
            s.Paciente.NumeroProntuario,
            s.Internacao.Enfermaria,
            s.CarimboDataHora,
            s.DataColeta,
            r.DataResultado,
            r.TesteRapido,
            r.Cultura,
            r.CepaIdentificada,
            r.ObservacoesLaboratorio,
            r.AssinaturaBase64,
            r.AssinadoPorNome,
            r.AssinadoEm,
            _plataforma.Nome,
            _plataforma.Laboratorio);
    }

    public async Task<(string Nome, string ContentType, byte[] Bytes)> BaixarAnexoAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var r = await db.ResultadosLaboratoriais
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SolicitacaoExameId == id, ct)
            ?? throw new NotFoundException("Resultado não encontrado.");

        if (r.LaudoAnexoBytes is null || r.LaudoAnexoBytes.Length == 0)
            throw new NotFoundException("Nenhum laudo anexado.");

        return (r.LaudoAnexoNome ?? "laudo.pdf", r.LaudoAnexoContentType ?? "application/pdf", r.LaudoAnexoBytes);
    }

    private async Task<SolicitacaoDetalheDto> RecarregarDetalheAsync(Guid id, CancellationToken ct)
    {
        var atualizada = await CarregarDetalheQuery().FirstAsync(s => s.Id == id, ct);
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

    private SolicitacaoDetalheDto MapDetalhe(SolicitacaoExame s)
    {
        var ocultarFicha = currentUser.Perfil == PerfilUsuario.Laboratorio;
        return new(
            s.Id,
            s.IdAmostraUnico,
            s.Status,
            s.CarimboDataHora,
            s.DataColeta,
            s.DataRecebimentoLaboratorio,
            s.QualidadeAmostra,
            s.Paciente.Nome,
            s.Paciente.NumeroProntuario,
            s.Internacao.Enfermaria,
            ocultarFicha || s.FormularioClinico == null ? null : new FormularioClinicoDto(
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
                s.ResultadoLaboratorial.AlertaPositivoEnviado,
                s.ResultadoLaboratorial.AssinadoPorNome,
                s.ResultadoLaboratorial.AssinadoEm,
                !string.IsNullOrWhiteSpace(s.ResultadoLaboratorial.AssinaturaBase64),
                s.ResultadoLaboratorial.LaudoAnexoNome,
                s.ResultadoLaboratorial.LaudoGeradoEm));
    }

    private async Task<string> GerarIdAmostraAsync(CancellationToken ct)
    {
        var hoje = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefixo = $"CDIF-{hoje}-";

        var ultimo = await db.SolicitacoesExame
            .Where(s => s.IdAmostraUnico.StartsWith(prefixo) || s.IdAmostraUnico.StartsWith($"ELO-{hoje}-"))
            .OrderByDescending(s => s.IdAmostraUnico)
            .Select(s => s.IdAmostraUnico)
            .FirstOrDefaultAsync(ct);

        var sequencia = 1;
        if (ultimo != null)
        {
            var idx = ultimo.LastIndexOf('-');
            if (idx >= 0 && int.TryParse(ultimo[(idx + 1)..], out var n))
                sequencia = n + 1;
        }

        return $"{prefixo}{sequencia:D4}";
    }

    private static string? NormalizarAssinatura(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        if (raw.Length > 400_000)
            throw new ValidationAppException("Assinatura muito grande.");

        return raw.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)
            ? raw
            : $"data:image/png;base64,{raw}";
    }
}
