using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a user in the system. Extends ASP.NET Core Identity.
/// Users always belong to a tenant (company).
/// </summary>
public class ApplicationUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfilePhotoPath { get; set; }
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Reference to the ASP.NET Core Identity user ID.
    /// This links the domain entity to the Identity system.
    /// </summary>
    public string IdentityUserId { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
