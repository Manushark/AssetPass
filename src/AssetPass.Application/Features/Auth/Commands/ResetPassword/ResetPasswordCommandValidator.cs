using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token de restablecimiento es requerido.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches(@"\d").WithMessage("La contraseña debe contener al menos un número.");

        RuleFor(x => x.NewPasswordConfirm)
            .NotEmpty().WithMessage("La confirmación de contraseña es requerida.")
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden.");
    }
}

