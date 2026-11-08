using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;

namespace AssetPass.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// Handles password change for an authenticated user.
/// Requires current password verification. Revokes all sessions after change.
/// </summary>
public sealed class ChangePasswordCommandHandler
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IValidator<ChangePasswordCommand> _validator;

    public ChangePasswordCommandHandler(
        IIdentityService identityService,
        IRefreshTokenStore refreshTokenStore,
        IValidator<ChangePasswordCommand> validator)
    {
        _identityService = identityService;
        _refreshTokenStore = refreshTokenStore;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(
        ChangePasswordCommand command,
        string authenticatedIdentityUserId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(authenticatedIdentityUserId))
            return Result.Unauthorized("No autenticado.");

        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var valErrors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result.Failure(valErrors);
        }

        var (success, changeErrors) = await _identityService.ChangePasswordAsync(
            authenticatedIdentityUserId,
            command.CurrentPassword,
            command.NewPassword,
            ct);

        if (!success)
            return Result.Failure(changeErrors);

        // Revoke all sessions after password change — user must re-login
        await _refreshTokenStore.RevokeAllAsync(authenticatedIdentityUserId, ct);

        return Result.Success();
    }
}

