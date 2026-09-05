using Elo.Application.DTOs.Auth;
using Elo.Domain.Enums;
using FluentValidation;

namespace Elo.Application.Validators;

public class SolicitarAcessoValidator : AbstractValidator<SolicitarAcessoRequest>
{
    public SolicitarAcessoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PerfilSolicitado)
            .Must(p => p is PerfilUsuario.Medico or PerfilUsuario.Laboratorio
                or PerfilUsuario.CCIH or PerfilUsuario.Enfermagem)
            .WithMessage("Selecione um perfil válido (médico, laboratório, CCIH ou enfermagem).");
        RuleFor(x => x.Setor).MaximumLength(120);
        RuleFor(x => x.Justificativa).MaximumLength(1000);
    }
}
