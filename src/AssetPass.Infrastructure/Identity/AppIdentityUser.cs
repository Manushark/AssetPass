using Microsoft.AspNetCore.Identity;

namespace AssetPass.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity user entity.
/// Linked to the domain ApplicationUser via IdentityUserId.
/// </summary>
public class AppIdentityUser : IdentityUser
{
    public Guid TenantId { get; set; }
    public Guid ApplicationUserId { get; set; }
}
