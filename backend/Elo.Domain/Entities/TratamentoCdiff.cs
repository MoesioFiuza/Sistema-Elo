using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class TratamentoCdiff : EntityBase
{
    public Guid SolicitacaoExameId { get; set; }
    public SolicitacaoExame SolicitacaoExame { get; set; } = null!;

    public SimNaoNaoRegistrado IniciouTratamento { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public DateTime? DataInicioTratamento { get; set; }
    public string? Medicacao { get; set; }
    public string? Dose { get; set; }
    public int? DuracaoDias { get; set; }

    public RespostaClinica RespostaDia7 { get; set; } = RespostaClinica.NaoRegistrado;
    public RespostaClinica RespostaFinal { get; set; } = RespostaClinica.NaoRegistrado;
    public SimNaoNaoRegistrado Recidiva { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public DateTime? DataRecidiva { get; set; }
    public string? ObservacoesTratamento { get; set; }
}
