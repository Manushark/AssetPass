namespace AssetPass.Application.Features.Users.Commands.InviteUser;

public sealed class InviteUserResponse
{
    public Guid InvitationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
