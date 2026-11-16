using FluentValidation;

namespace AssetPass.Application.Features.Users.Commands.InviteUser;

public sealed class InviteUserCommandValidator : AbstractValidator<InviteUserCommand>
{
    public InviteUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
            .MaximumLength(200).WithMessage("El correo no puede exceder 200 caracteres.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("El rol especificado no es válido.")
            .NotEqual(Domain.Enums.UserRole.Owner).WithMessage("No se puede invitar a un usuario con el rol de Owner.");
    }
}
