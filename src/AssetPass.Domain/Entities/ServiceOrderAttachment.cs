using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// File attachment for a service order (photos, documents).
/// </summary>
public class ServiceOrderAttachment : TenantEntity
{
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    // Foreign keys
    public Guid ServiceOrderId { get; set; }

    // Navigation properties
    public ServiceOrder ServiceOrder { get; set; } = null!;
}
