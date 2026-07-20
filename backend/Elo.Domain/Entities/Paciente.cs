using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class Paciente : EntityBase
{
    public string NumeroProntuario { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public Sexo Sexo { get; set; }

    // Histórico diarreia / C. diff
    public SimNaoNaoRegistrado HistoricoDiarreiaPrevia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado DiarreiaAssociadaAtbPassado { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado ProcurouAtendimentoPorDiarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado InternadoPorDiarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? QuandoInternadoPorDiarreia { get; set; }
    public SimNaoNaoRegistrado HistoricoCdiff { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado CdiffFamiliaAmbiente { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? ProblemasSaudeAdjacentes { get; set; }
    public string? ProblemasSaudeOutros { get; set; }

    public SimNaoNaoRegistrado HistoricoTransplante { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado HistoricoQuimioterapia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    // COVID
    public SimNaoNaoRegistrado HistoricoCovid { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? CovidAnosPositivos { get; set; }
    public SimNaoNaoRegistrado CovidTeveSintomas { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? CovidSintomasDescricao { get; set; }
    public SimNaoNaoRegistrado CovidInternado { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public int? CovidDiasInternacao { get; set; }
    public SimNaoNaoRegistrado CovidOxigenioOuTratamentos { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? CovidTratamentosDescricao { get; set; }
    public SimNaoNaoRegistrado CovidIntubado { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? CovidQuandoIntubacao { get; set; }
    public int? CovidDiasIntubado { get; set; }
    public SimNaoNaoRegistrado CovidUtiDuranteIntubacao { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    public ICollection<Internacao> Internacoes { get; set; } = [];
    public ICollection<SolicitacaoExame> Solicitacoes { get; set; } = [];
}
