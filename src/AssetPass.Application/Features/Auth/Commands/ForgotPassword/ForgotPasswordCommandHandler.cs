using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Handles the forgot password flow.
/// Generates a reset token and delivers it via email.
///
/// Security: Always returns success even if the email doesn't exist
/// to prevent user enumeration attacks.
/// </summary>
public sealed class ForgotPasswordCommandHandler
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IValidator<ForgotPasswordCommand> _validator;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        IValidator<ForgotPasswordCommand> validator)
    {
        _identityService = identityService;
        _emailService = emailService;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(ForgotPasswordCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result.Failure(errors);
        }

        var email = command.Email.Trim().ToLowerInvariant();

        // Generate token — returns null if email not found (we still return success)
        var token = await _identityService.GeneratePasswordResetTokenAsync(email, ct);

        if (token is not null)
        {
            // In production: build a frontend URL with token param (e.g. /reset-password?token=...&email=...)
            // For MVP: send raw token so the frontend can encode it in the URL
            var resetLink = $"reset-token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
            await _emailService.SendPasswordResetEmailAsync(email, resetLink, ct);
        }

        // Always succeed — prevents user enumeration
        return Result.Success();
    }
}
