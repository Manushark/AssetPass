namespace AssetPass.Application.Features.Auth.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommand
{
    public string Token { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}
