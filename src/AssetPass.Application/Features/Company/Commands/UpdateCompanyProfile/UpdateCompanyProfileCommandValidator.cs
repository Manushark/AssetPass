using FluentValidation;

namespace AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;

public sealed class UpdateCompanyProfileCommandValidator : AbstractValidator<UpdateCompanyProfileCommand>
{
    public UpdateCompanyProfileCommandValidator()
    {
        RuleFor(x => x.CommercialName)
            .NotEmpty().WithMessage("El nombre comercial es requerido.")
            .MaximumLength(200).WithMessage("El nombre comercial no puede exceder 200 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es requerido.")
            .MaximumLength(50).WithMessage("El teléfono no puede exceder 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.")
            .MaximumLength(200).WithMessage("El correo no puede exceder 200 caracteres.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("La moneda debe ser un código de 3 caracteres (ej. DOP, USD).");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("La zona horaria es requerida.");

        RuleFor(x => x.LegalName)
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres.");

        RuleFor(x => x.Rnc)
            .MaximumLength(50).WithMessage("El RNC no puede exceder 50 caracteres.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres.");
    }
}
