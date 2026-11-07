namespace AssetPass.Application.Interfaces;

/// <summary>
/// Abstraction over the ASP.NET Core Identity operations needed by the Application layer.
/// This prevents Application from taking a direct dependency on Identity or Infrastructure.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Creates a new Identity user and assigns the given role.
    /// Returns (identityUserId, errors[]).
    /// </summary>
    Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        Guid tenantId,
        Guid applicationUserId,
        string role,
        CancellationToken ct = default);

    /// <summary>
    /// Verifies credentials and returns the identity user data on success, or null on failure.
    /// Applies lockout on repeated failures.
    /// </summary>
    Task<IdentityUserData?> CheckPasswordAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Finds an identity user by their email address or identity user ID.
    /// </summary>
    Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Generates a password reset token for the given email.
    /// Returns null if the email is not found.
    /// </summary>
    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Resets the user's password using a valid token (from GeneratePasswordResetTokenAsync).
    /// Returns (success, errors[]).
    /// </summary>
    Task<(bool Success, string[] Errors)> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default);

    /// <summary>
    /// Changes the authenticated user's password after verifying the current password.
    /// Returns (success, errors[]).
    /// </summary>
    Task<(bool Success, string[] Errors)> ChangePasswordAsync(
        string identityUserId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default);
    /// <summary>
    /// Changes the role of an Identity user (removes the old role and assigns the new one).
    /// </summary>
    Task ChangeUserRoleAsync(
        string identityUserId,
        string oldRole,
        string newRole,
        CancellationToken ct = default);
}

/// <summary>
/// Lightweight DTO carrying only the data the Application layer needs from an Identity user.
/// This avoids exposing the full IdentityUser type across layer boundaries.
/// </summary>
public sealed class IdentityUserData
{
    public string IdentityUserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid TenantId { get; init; }
    public Guid ApplicationUserId { get; init; }
    public string Role { get; init; } = string.Empty;
}
