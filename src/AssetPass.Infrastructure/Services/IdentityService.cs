using AssetPass.Application.Interfaces;
using AssetPass.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace AssetPass.Infrastructure.Services;

/// <summary>
/// Implements IIdentityService using ASP.NET Core Identity (UserManager, SignInManager).
/// This class lives in Infrastructure — the Application layer depends only on the interface.
/// </summary>
public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly SignInManager<AppIdentityUser> _signInManager;

    public IdentityService(
        UserManager<AppIdentityUser> userManager,
        SignInManager<AppIdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <inheritdoc />
    public async Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        Guid tenantId,
        Guid applicationUserId,
        string role,
        CancellationToken ct = default)
    {
        var user = new AppIdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true, // auto-confirm until email verification is implemented
            TenantId = tenantId,
            ApplicationUserId = applicationUserId
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToArray();
            return (null, errors);
        }

        // Assign role — role must exist in the database (seeded via EF migration or startup)
        await _userManager.AddToRoleAsync(user, role);

        return (user.Id, []);
    }

    /// <inheritdoc />
    public async Task<IdentityUserData?> CheckPasswordAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        // Use CheckPasswordSignInAsync to benefit from lockout tracking
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? string.Empty;

        return new IdentityUserData
        {
            IdentityUserId = user.Id,
            Email = user.Email ?? email,
            TenantId = user.TenantId,
            ApplicationUserId = user.ApplicationUserId,
            Role = primaryRole
        };
    }

    /// <inheritdoc />
    public async Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        // Also support lookup by identity user ID (used by RefreshTokenHandler)
        AppIdentityUser? user;
        if (Guid.TryParse(email, out _) || (!email.Contains('@') && email.Length > 20))
            user = await _userManager.FindByIdAsync(email);
        else
            user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new IdentityUserData
        {
            IdentityUserId = user.Id,
            Email = user.Email ?? string.Empty,
            TenantId = user.TenantId,
            ApplicationUserId = user.ApplicationUserId,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
    }
}

