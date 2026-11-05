namespace AssetPass.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Initiates a password reset flow by generating a reset token and sending it via email.
/// Always returns success regardless of whether the email exists (prevents user enumeration).
/// </summary>
public sealed class ForgotPasswordCommand
{
    public string Email { get; init; } = string.Empty;
}

