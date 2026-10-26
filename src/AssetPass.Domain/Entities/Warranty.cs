using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a warranty associated with a piece of equipment.
/// An equipment can have multiple warranties (original, extended, etc.).
/// </summary>
public class Warranty : TenantEntity
{
    public string Provider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Conditions { get; set; }
    public string? DocumentPath { get; set; }
    public WarrantyStatus Status { get; set; } = WarrantyStatus.Active;
    public string? Notes { get; set; }

    // Foreign keys
    public Guid EquipmentId { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;

    /// <summary>
    /// Whether the warranty is currently active based on dates.
    /// </summary>
    public bool IsCurrentlyActive => Status == WarrantyStatus.Active && EndDate > DateTime.UtcNow;

    /// <summary>
    /// Whether the warranty is expiring within the next 30 days.
    /// </summary>
    public bool IsExpiringSoon => Status == WarrantyStatus.Active &&
                                  EndDate > DateTime.UtcNow &&
                                  EndDate <= DateTime.UtcNow.AddDays(30);
}
