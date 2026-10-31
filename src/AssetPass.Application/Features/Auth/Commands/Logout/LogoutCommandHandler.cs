using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;

namespace AssetPass.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handles user logout and refresh token revocation.
/// Supports both single-session logout (current refresh token) and global logout (all user sessions).
/// </summary>
public sealed class LogoutCommandHandler
{
    private readonly IRefreshTokenStore _refreshTokenStore;

    public LogoutCommandHandler(IRefreshTokenStore refreshTokenStore)
    {
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<Result> HandleAsync(
        LogoutCommand command,
        string? authenticatedIdentityUserId = null,
        CancellationToken ct = default)
    {
        string? targetUserId = authenticatedIdentityUserId;

        // If not authenticated via JWT, resolve user from the refresh token
        if (string.IsNullOrEmpty(targetUserId) && !string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            targetUserId = await _refreshTokenStore.ValidateAndGetUserIdAsync(command.RefreshToken, ct);
        }

        // If neither active auth context nor refresh token was provided
        if (string.IsNullOrEmpty(targetUserId) && string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure("Se requiere un refresh token o una sesión activa para cerrar sesión.");
        }

        // Revoke tokens
        if (command.RevokeAllSessions && !string.IsNullOrEmpty(targetUserId))
        {
            await _refreshTokenStore.RevokeAllAsync(targetUserId, ct);
        }
        else if (!string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            await _refreshTokenStore.RevokeTokenAsync(command.RefreshToken, ct);
        }
        else if (!string.IsNullOrEmpty(targetUserId))
        {
            await _refreshTokenStore.RevokeAllAsync(targetUserId, ct);
        }

        return Result.Success();
    }
}
