using Elo.Application.DTOs.Pacientes;
using FluentValidation;

namespace Elo.Application.Validators;

public class CreatePacienteValidator : AbstractValidator<CreatePacienteRequest>
{
    public CreatePacienteValidator()
    {
        RuleFor(x => x.NumeroProntuario)
            .NotEmpty().WithMessage("Número do prontuário é obrigatório.")
            .MaximumLength(50);

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(300);

        RuleFor(x => x.Enfermaria)
            .NotEmpty().WithMessage("Enfermaria é obrigatória para internação.")
            .MaximumLength(100);
    }
}
