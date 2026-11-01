namespace AssetPass.Application.Interfaces;

/// <summary>
/// Abstraction over the refresh token persistence store.
/// Infrastructure implements this; Application only depends on the interface.
/// </summary>
public interface IRefreshTokenStore
{
    /// <summary>
    /// Persists a new refresh token for a given identity user.
    /// </summary>
    Task StoreAsync(string identityUserId, string refreshToken, DateTime expiresAt, CancellationToken ct = default);

    /// <summary>
    /// Validates a refresh token and returns the associated identity user ID, or null if invalid/expired.
    /// </summary>
    Task<string?> ValidateAndGetUserIdAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Revokes all active refresh tokens for a user (e.g., on logout or password change).
    /// </summary>
    Task RevokeAllAsync(string identityUserId, CancellationToken ct = default);
}

