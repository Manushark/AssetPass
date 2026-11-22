namespace AssetPass.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command to exchange a valid refresh token for new JWT tokens.
/// </summary>
public sealed class RefreshTokenCommand
{
    public string RefreshToken { get; init; } = string.Empty;
}

