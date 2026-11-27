using FluentValidation;

namespace AssetPass.Application.Features.Clients.Commands.CreateClient;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del cliente es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.ClientType)
            .IsInEnum().WithMessage("Tipo de cliente inválido.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
            .MaximumLength(200).WithMessage("El correo no puede exceder 200 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("El teléfono no puede exceder 50 caracteres.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Las notas no pueden exceder 2000 caracteres.");
    }
}
