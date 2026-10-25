using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a company (tenant) in the system.
/// This is the root entity for multi-tenancy isolation.
/// </summary>
public class Tenant : BaseEntity
{
    public string CommercialName { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Rnc { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LogoPath { get; set; }
    public string Currency { get; set; } = "DOP";
    public string TimeZone { get; set; } = "America/Santo_Domingo";
    public string Locale { get; set; } = "es-DO";

    // Subscription
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public Guid? PlanId { get; set; }
    public Plan? Plan { get; set; }
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    public ICollection<EquipmentCategory> EquipmentCategories { get; set; } = new List<EquipmentCategory>();

    // Public portal configuration
    public bool ShowWarrantyInPublicPortal { get; set; } = true;
    public bool ShowMaintenanceHistoryInPublicPortal { get; set; } = true;
    public bool ShowNextMaintenanceInPublicPortal { get; set; } = true;
    public bool ShowLastMaintenanceInPublicPortal { get; set; } = true;
    public bool ShowEquipmentPhotoInPublicPortal { get; set; } = true;
    public bool ShowCompanyInfoInPublicPortal { get; set; } = true;
}
