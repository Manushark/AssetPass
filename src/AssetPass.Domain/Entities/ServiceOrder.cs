using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a work order for service to be performed on equipment.
/// When completed, a corresponding service record should be generated.
/// </summary>
public class ServiceOrder : TenantEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public ServiceType ServiceType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Observations { get; set; }
    public string? MaterialsUsed { get; set; }
    public string? Result { get; set; }
    public int Priority { get; set; } = 0;

    // Foreign keys
    public Guid EquipmentId { get; set; }
    public Guid ClientId { get; set; }
    public Guid? EstablishmentId { get; set; }
    public Guid? AssignedTechnicianId { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Establishment? Establishment { get; set; }
    public ApplicationUser? AssignedTechnician { get; set; }
    public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
    public ICollection<ServiceOrderAttachment> Attachments { get; set; } = new List<ServiceOrderAttachment>();

    /// <summary>
    /// Idempotency key to prevent duplicate order creation on retry.
    /// </summary>
    public string? IdempotencyKey { get; set; }
}
