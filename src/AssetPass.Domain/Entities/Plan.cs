using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Subscription plan definition.
/// Plans define limits and pricing for tenant subscriptions.
/// </summary>
public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPriceDop { get; set; }
    public string Currency { get; set; } = "DOP";
    public int MaxUsers { get; set; }
    public int MaxEquipment { get; set; }
    public int MaxClients { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public int TrialDays { get; set; } = 14;

    // Features
    public bool HasApiAccess { get; set; }
    public bool HasAdvancedReporting { get; set; }
    public bool HasCustomBranding { get; set; }
    public bool HasPrioritySupport { get; set; }

    // Navigation
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
