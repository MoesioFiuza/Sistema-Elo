using Elo.Application.DTOs.Solicitacoes;
using FluentValidation;

namespace Elo.Application.Validators;

public class CreateSolicitacaoValidator : AbstractValidator<CreateSolicitacaoRequest>
{
    public CreateSolicitacaoValidator()
    {
        RuleFor(x => x.PacienteId).NotEmpty();
        RuleFor(x => x.InternacaoId).NotEmpty();
        RuleFor(x => x.Formulario).NotNull();
        RuleFor(x => x.Formulario.Diarreia)
            .Equal(Domain.Enums.SimNaoNaoRegistrado.Sim)
            .WithMessage("Confirme diarreia (pelo menos 3 episódios em 24h).");
        RuleFor(x => x.Formulario.EpisodiosDiarreia24h)
            .NotNull()
            .GreaterThanOrEqualTo(3)
            .WithMessage("Informe pelo menos 3 episódios de diarreia em 24 horas.");
        RuleFor(x => x.Formulario.ConsistenciaFezes)
            .Must(c => c is Domain.Enums.ConsistenciaFezes.Liquida or Domain.Enums.ConsistenciaFezes.Pastosa)
            .WithMessage("Informe o aspecto das fezes (líquido ou pastoso).");
    }
}

public class RegistrarResultadoValidator : AbstractValidator<RegistrarResultadoRequest>
{
    public RegistrarResultadoValidator()
    {
        RuleFor(x => x.TesteRapido)
            .NotEqual(Domain.Enums.ResultadoTeste.NaoRegistrado)
            .WithMessage("Informe o resultado do teste rápido.");
        RuleFor(x => x.Cultura)
            .NotEqual(Domain.Enums.ResultadoTeste.NaoRegistrado)
            .WithMessage("Informe o resultado da cultura.");
        RuleFor(x => x.AssinaturaBase64)
            .NotEmpty()
            .WithMessage("A assinatura do responsável é obrigatória.");
        RuleFor(x => x.AssinadoPorNome)
            .NotEmpty()
            .WithMessage("Informe o nome de quem assina o laudo.");
    }
}
