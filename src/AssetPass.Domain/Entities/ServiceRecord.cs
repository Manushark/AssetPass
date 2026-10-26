using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a single entry in an equipment's service history.
/// This is an immutable record — once created, it should not be modified.
/// Corrections should be made by creating a new record with a reference to the original.
/// </summary>
public class ServiceRecord : TenantEntity
{
    public ServiceType ServiceType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Observations { get; set; }
    public DateTime ServiceDate { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime? NextServiceDate { get; set; }

    /// <summary>
    /// Whether this record should be visible in the public equipment portal.
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Private notes for technicians, never shown in public portal.
    /// </summary>
    public string? PrivateNotes { get; set; }

    // Foreign keys
    public Guid EquipmentId { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid? ServiceOrderId { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;
    public ApplicationUser? Technician { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }
    public ICollection<ServiceRecordAttachment> Attachments { get; set; } = new List<ServiceRecordAttachment>();
}
