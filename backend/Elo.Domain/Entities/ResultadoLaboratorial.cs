using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

public class ResultadoLaboratorial : EntityBase
{
    public Guid SolicitacaoExameId { get; set; }
    public SolicitacaoExame SolicitacaoExame { get; set; } = null!;

    public Guid? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }

    public DateTime DataResultado { get; set; }
    public ResultadoTeste TesteRapido { get; set; } = ResultadoTeste.NaoRegistrado;
    public ResultadoTeste ToxinaA { get; set; } = ResultadoTeste.NaoRegistrado;
    public ResultadoTeste ToxinaB { get; set; } = ResultadoTeste.NaoRegistrado;
    public ResultadoTeste Cultura { get; set; } = ResultadoTeste.NaoRegistrado;
    public string? CepaIdentificada { get; set; }
    public string? ObservacoesLaboratorio { get; set; }

    public bool AlertaPositivoEnviado { get; set; }
    public DateTime? DataAlertaEnviado { get; set; }
    public bool LiberacaoIsolamentoEnviada { get; set; }
    public DateTime? DataLiberacaoIsolamento { get; set; }
}
