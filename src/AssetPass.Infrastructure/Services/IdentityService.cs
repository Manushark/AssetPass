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
            EmailConfirmed = true,
            TenantId = tenantId,
            ApplicationUserId = applicationUserId
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToArray();
            return (null, errors);
        }

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

    /// <inheritdoc />
    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string[] Errors)> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, ["Token de restablecimiento inválido o expirado."]);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        return (true, []);
    }

    /// <inheritdoc />
    public async Task<(bool Success, string[] Errors)> ChangePasswordAsync(
        string identityUserId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
            return (false, ["Usuario no encontrado."]);

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        return (true, []);
    }

    /// <inheritdoc />
    public async Task ChangeUserRoleAsync(
        string identityUserId,
        string oldRole,
        string newRole,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null) return;

        if (await _userManager.IsInRoleAsync(user, oldRole))
            await _userManager.RemoveFromRoleAsync(user, oldRole);

        await _userManager.AddToRoleAsync(user, newRole);
    }
}
