namespace AssetPass.Application.Features.Auth.Commands.Login;

/// <summary>
/// Returned after a successful login with all tokens and user context.
/// </summary>
public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

