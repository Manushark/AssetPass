using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handles user authentication and JWT token issuance.
///
/// Orchestration:
/// 1. Validate the command.
/// 2. Verify credentials via IIdentityService (applies lockout on failure).
/// 3. Load the domain ApplicationUser to get name and role details.
/// 4. Generate access token and refresh token.
/// 5. Persist the refresh token.
/// 6. Return LoginResponse.
///
/// Security: email is case-folded; lockout is enforced by Identity.
/// </summary>
public sealed class LoginCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        IApplicationDbContext db,
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenStore refreshTokenStore,
        IValidator<LoginCommand> validator)
    {
        _db = db;
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenStore = refreshTokenStore;
        _validator = validator;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken ct = default)
    {
        // 1. Validate
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToArray();
            return Result<LoginResponse>.Failure(errors);
        }

        // 2. Check password (also handles lockout)
        var identityUser = await _identityService.CheckPasswordAsync(
            command.Email.Trim().ToLowerInvariant(),
            command.Password,
            ct);

        if (identityUser is null)
            return Result<LoginResponse>.Failure("Credenciales incorrectas.");

        // 3. Load domain user to get display info
        var domainUser = await _db.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.IdentityUserId, ct);

        if (domainUser is null || !domainUser.IsActive)
            return Result<LoginResponse>.Failure("La cuenta de usuario no está activa.");

        // 4. Generate tokens
        var expirationMinutes = _jwtTokenService.GetAccessTokenExpirationMinutes();
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            identityUserId: identityUser.IdentityUserId,
            email: identityUser.Email,
            tenantId: identityUser.TenantId,
            applicationUserId: domainUser.Id,
            role: domainUser.Role.ToString());

        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // 5. Persist refresh token (30-day expiry)
        await _refreshTokenStore.StoreAsync(
            identityUserId: identityUser.IdentityUserId,
            refreshToken: refreshToken,
            expiresAt: DateTime.UtcNow.AddDays(30),
            ct: ct);

        // 6. Return response
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = domainUser.Id,
            TenantId = identityUser.TenantId,
            Role = domainUser.Role.ToString(),
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            Email = domainUser.Email
        };

        return Result<LoginResponse>.Success(response);
    }
}

