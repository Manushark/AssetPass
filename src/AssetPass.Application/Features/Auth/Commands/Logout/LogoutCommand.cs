namespace AssetPass.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to logout a user session and revoke token(s).
/// </summary>
public sealed class LogoutCommand
{
    /// <summary>
    /// The refresh token to be revoked. Optional if authenticated via Bearer token.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// If true, revokes all active refresh tokens for the user across all devices/sessions.
    /// Default is false (only the current session/token is revoked).
    /// </summary>
    public bool RevokeAllSessions { get; init; } = false;
}
