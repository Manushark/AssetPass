namespace AssetPass.Domain.Common;

/// <summary>
/// Base entity with common audit properties for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Base entity for all tenant-scoped entities.
/// Every entity that belongs to a specific company must inherit from this.
/// </summary>
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
