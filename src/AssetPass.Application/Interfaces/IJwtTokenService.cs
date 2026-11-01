namespace AssetPass.Application.Interfaces;

/// <summary>
/// Abstraction over the JWT and refresh token generation infrastructure.
/// Defined in Application to avoid coupling handlers directly to the API layer.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed JWT access token for an authenticated identity user.
    /// </summary>
    string GenerateAccessToken(string identityUserId, string email, Guid tenantId, Guid applicationUserId, string role);

    /// <summary>
    /// Generates a cryptographically random refresh token string.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Returns how many minutes the access token is valid for.
    /// </summary>
    int GetAccessTokenExpirationMinutes();
}

