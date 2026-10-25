using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Core entity of AssetPass. Represents a piece of equipment
/// with its digital passport (public identifier and QR code).
/// </summary>
public class Equipment : TenantEntity
{
    // Identification
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Secure, non-sequential public identifier for QR code URLs.
    /// Generated using NanoID (21 chars, URL-safe alphabet).
    /// </summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>
    /// Whether the public QR link is currently active.
    /// When revoked, scanning the QR shows a "link revoked" message.
    /// </summary>
    public bool IsPublicLinkActive { get; set; } = true;

    // Specifications
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;

    // Dates
    public DateTime? InstallationDate { get; set; }
    public DateTime? AcquisitionDate { get; set; }

    // Media
    public string? PhotoPath { get; set; }

    // Foreign keys
    public Guid ClientId { get; set; }
    public Guid? EstablishmentId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public Client Client { get; set; } = null!;
    public Establishment? Establishment { get; set; }
    public EquipmentCategory Category { get; set; } = null!;
    public ICollection<ServiceRecord> ServiceHistory { get; set; } = new List<ServiceRecord>();
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
}
