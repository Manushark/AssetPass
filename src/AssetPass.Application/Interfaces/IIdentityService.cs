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
    /// Finds an identity user by their email address.
    /// </summary>
    Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default);
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

