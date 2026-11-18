using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token de invitación es requerido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");
    }
}
