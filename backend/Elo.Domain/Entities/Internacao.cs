using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class Internacao : EntityBase
{
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public string Enfermaria { get; set; } = string.Empty;
    public string? Leito { get; set; }
    public DateTime DataInternacao { get; set; }
    public DateTime? DataAlta { get; set; }
    public string? MotivoInternacao { get; set; }
    public TipoCirurgia TipoCirurgia { get; set; } = TipoCirurgia.NaoAplicavel;
    public SimNaoNaoRegistrado ParaTcth { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado ParaTos { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    public SimNaoNaoRegistrado InternouComDiarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado UsoImunossupressoresDurante { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado UsoImunossupressoresAtual { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? ImunossupressoresDescricao { get; set; }

    public SimNaoNaoRegistrado EmUti { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Leucocitose { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Leucopenia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Sepse { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Obito { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public DateTime? DataObito { get; set; }

    /// <summary>Paciente sob isolamento por resultado positivo.</summary>
    public bool IsolamentoAtivo { get; set; }

    public ICollection<SolicitacaoExame> Solicitacoes { get; set; } = [];
}
