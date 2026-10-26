using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a preventive maintenance schedule for equipment.
/// The system uses this to detect upcoming and overdue maintenance.
/// </summary>
public class MaintenanceSchedule : TenantEntity
{
    /// <summary>
    /// Frequency in days between maintenance visits.
    /// </summary>
    public int FrequencyDays { get; set; }

    public DateTime? NextMaintenanceDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid EquipmentId { get; set; }
    public Guid? AssignedTechnicianId { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;
    public ApplicationUser? AssignedTechnician { get; set; }

    /// <summary>
    /// Whether maintenance is overdue (past the scheduled date).
    /// </summary>
    public bool IsOverdue => IsActive &&
                              NextMaintenanceDate.HasValue &&
                              NextMaintenanceDate.Value < DateTime.UtcNow;

    /// <summary>
    /// Whether maintenance is due within the next 7 days.
    /// </summary>
    public bool IsDueSoon => IsActive &&
                              NextMaintenanceDate.HasValue &&
                              NextMaintenanceDate.Value >= DateTime.UtcNow &&
                              NextMaintenanceDate.Value <= DateTime.UtcNow.AddDays(7);
}
