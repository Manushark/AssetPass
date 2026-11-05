namespace AssetPass.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user and receive JWT tokens.
/// </summary>
public sealed class LoginCommand
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

