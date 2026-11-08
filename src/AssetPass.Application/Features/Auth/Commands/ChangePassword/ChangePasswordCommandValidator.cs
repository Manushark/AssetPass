using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La contraseña actual es requerida.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una letra minúscula.")
            .Matches(@"\d").WithMessage("Debe contener al menos un número.")
            .NotEqual(x => x.CurrentPassword).WithMessage("La nueva contraseña no puede ser igual a la actual.");

        RuleFor(x => x.NewPasswordConfirm)
            .NotEmpty().WithMessage("La confirmación de contraseña es requerida.")
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden.");
    }
}

