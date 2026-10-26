using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Tracks user invitations sent by tenant administrators.
/// An invitation creates a pending user record that can be accepted.
/// </summary>
public class UserInvitation : TenantEntity
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Domain.Enums.UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public Guid? InvitedByUserId { get; set; }

    public bool IsExpired => !IsAccepted && ExpiresAt < DateTime.UtcNow;
}
