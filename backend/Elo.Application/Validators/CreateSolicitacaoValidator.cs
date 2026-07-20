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
    }
}

public class RegistrarResultadoValidator : AbstractValidator<RegistrarResultadoRequest>
{
    public RegistrarResultadoValidator()
    {
        RuleFor(x => x.TesteRapido)
            .NotEqual(Domain.Enums.ResultadoTeste.NaoRegistrado)
            .WithMessage("Informe o resultado do teste rápido.");
    }
}
