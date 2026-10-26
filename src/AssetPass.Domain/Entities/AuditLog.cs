using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Audit log entry for tracking important operations.
/// Provides traceability for security and compliance.
/// </summary>
public class AuditLog : TenantEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
