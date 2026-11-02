using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.RegisterTenant;

/// <summary>
/// Validates a RegisterTenantCommand before execution.
/// Ensures data integrity at the application boundary.
/// </summary>
public sealed class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantCommandValidator()
    {
        // Company rules
        RuleFor(x => x.CommercialName)
            .NotEmpty().WithMessage("El nombre comercial es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono de la empresa es requerido.")
            .MaximumLength(30);

        RuleFor(x => x.CompanyEmail)
            .NotEmpty().WithMessage("El correo de la empresa es requerido.")
            .EmailAddress().WithMessage("El correo de la empresa no es válido.")
            .MaximumLength(256);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("La moneda debe ser un código ISO de 3 caracteres.");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("La zona horaria es requerida.");

        // Admin user rules
        RuleFor(x => x.AdminFirstName)
            .NotEmpty().WithMessage("El nombre del administrador es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.AdminLastName)
            .NotEmpty().WithMessage("El apellido del administrador es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("El correo del administrador es requerido.")
            .EmailAddress().WithMessage("El correo del administrador no es válido.")
            .MaximumLength(256);

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches(@"\d").WithMessage("La contraseña debe contener al menos un número.");

        RuleFor(x => x.AdminPasswordConfirm)
            .NotEmpty().WithMessage("La confirmación de contraseña es requerida.")
            .Equal(x => x.AdminPassword).WithMessage("Las contraseñas no coinciden.");
    }
}

