using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.ResetPassword;

/// <summary>
/// Handles password reset using a token delivered to the user's email.
/// </summary>
public sealed class ResetPasswordCommandHandler
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordCommandHandler(
        IIdentityService identityService,
        IRefreshTokenStore refreshTokenStore,
        IValidator<ResetPasswordCommand> validator)
    {
        _identityService = identityService;
        _refreshTokenStore = refreshTokenStore;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var valErrors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result.Failure(valErrors);
        }

        var email = command.Email.Trim().ToLowerInvariant();

        var (success, resetErrors) = await _identityService.ResetPasswordAsync(
            email, command.Token, command.NewPassword, ct);

        if (!success)
            return Result.Failure(resetErrors);

        // After password reset: revoke all existing sessions for security
        var user = await _identityService.FindByEmailAsync(email, ct);
        if (user is not null)
            await _refreshTokenStore.RevokeAllAsync(user.IdentityUserId, ct);

        return Result.Success();
    }
}

