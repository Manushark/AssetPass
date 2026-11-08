namespace AssetPass.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// Changes the authenticated user's own password.
/// Requires the current password for verification.
/// </summary>
public sealed class ChangePasswordCommand
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string NewPasswordConfirm { get; init; } = string.Empty;
}

