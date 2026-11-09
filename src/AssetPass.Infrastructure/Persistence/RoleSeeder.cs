using AssetPass.Domain.Enums;
using AssetPass.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AssetPass.Infrastructure.Persistence;

/// <summary>
/// Seeds the Identity role table with the AssetPass role definitions.
/// Must be called on application startup after migrations are applied.
///
/// Roles are immutable system roles — tenant users are assigned one of these.
/// </summary>
public static class RoleSeeder
{
    /// <summary>
    /// All valid roles in the system, derived from the UserRole enum.
    /// </summary>
    public static readonly IReadOnlyList<string> AllRoles =
        Enum.GetNames<UserRole>();

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        foreach (var roleName in AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation("Role '{Role}' created successfully.", roleName);
                else
                    logger.LogWarning("Failed to create role '{Role}': {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}

