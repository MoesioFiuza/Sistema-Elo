using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class SolicitacaoExame : EntityBase
{
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public Guid InternacaoId { get; set; }
    public Internacao Internacao { get; set; } = null!;

    public Guid SolicitanteId { get; set; }
    public Usuario Solicitante { get; set; } = null!;

    public DateTime CarimboDataHora { get; set; }
    public string IdAmostraUnico { get; set; } = string.Empty;
    public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Pendente;

    public DateTime? DataColeta { get; set; }
    public DateTime? DataRecebimentoLaboratorio { get; set; }
    public QualidadeAmostra QualidadeAmostra { get; set; } = QualidadeAmostra.NaoAvaliada;
    public DateTime? DataAvaliacaoAmostra { get; set; }

    public FormularioClinico? FormularioClinico { get; set; }
    public ResultadoLaboratorial? ResultadoLaboratorial { get; set; }
    public TratamentoCdiff? TratamentoCdiff { get; set; }
}
