using FluentValidation;

namespace AssetPass.Application.Features.Establishments.Commands.UpdateEstablishment;

public sealed class UpdateEstablishmentCommandValidator : AbstractValidator<UpdateEstablishmentCommand>
{
    public UpdateEstablishmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del establecimiento es obligatorio.");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del establecimiento es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La dirección no puede superar los 500 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("El teléfono no puede superar los 50 caracteres.");

        RuleFor(x => x.ContactPerson)
            .MaximumLength(200).WithMessage("La persona de contacto no puede superar los 200 caracteres.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Las notas no pueden superar los 2000 caracteres.");
    }
}
