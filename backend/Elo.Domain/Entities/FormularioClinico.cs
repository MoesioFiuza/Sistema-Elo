using Elo.Domain.Common;
using Elo.Domain.Enums;

namespace Elo.Domain.Entities;

/// <summary>Ficha clínica/epidemiológica no momento da solicitação do exame.</summary>
public class FormularioClinico : EntityBase
{
    public Guid SolicitacaoExameId { get; set; }
    public SolicitacaoExame SolicitacaoExame { get; set; } = null!;

    // Diarreia / sintomas
    public SimNaoNaoRegistrado Diarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public int? DiasInicioSintomas { get; set; }
    public int? EpisodiosDiarreia24h { get; set; }
    public ConsistenciaFezes ConsistenciaFezes { get; set; } = ConsistenciaFezes.NaoRegistrado;
    public string? SintomasAssociados { get; set; }

    public SimNaoNaoRegistrado UsoIbpAntesDiarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado UsoIbpDuranteDiarreia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    /// <summary>Mantido por compatibilidade; preferir Antes/Durante.</summary>
    public SimNaoNaoRegistrado UsoIbp { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? IbpDescricao { get; set; }

    public SimNaoNaoRegistrado DorAbdominal { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Febre { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public decimal? TemperaturaMaxima { get; set; }
    public string? DuracaoFebre { get; set; }

    public SimNaoNaoRegistrado Peritonite { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado IleoParalitico { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Megacolon { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    public SimNaoNaoRegistrado VentilacaoMecanica { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado InternouUtiDurante { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Leucocitose { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado Leucopenia { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado FezIra { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado DrogasVasoativas { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado DesorientacaoConfusao { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    public SimNaoNaoRegistrado NutricaoParenteral { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public SimNaoNaoRegistrado SondaNasogastrica { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;

    // Antimicrobianos
    public SimNaoNaoRegistrado UsoAntimicrobianoAntesColeta { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? AntimicrobianosAntesDescricao { get; set; }
    public SimNaoNaoRegistrado UsoAntimicrobianoDiaColeta { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? AntimicrobianosDiaColetaDescricao { get; set; }
    /// <summary>Compatibilidade com campo antigo "30 dias".</summary>
    public SimNaoNaoRegistrado UsoAntimicrobiano30d { get; set; } = SimNaoNaoRegistrado.NaoRegistrado;
    public string? AntimicrobianosDescricao { get; set; }

    public string? ObservacoesClinicas { get; set; }
}
