namespace AssetPass.Application.Features.Users.Commands.InviteUser;

public sealed class InviteUserCommand
{
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public Domain.Enums.UserRole Role { get; init; }
}
