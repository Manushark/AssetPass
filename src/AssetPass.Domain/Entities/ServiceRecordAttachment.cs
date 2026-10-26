using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a file attachment (photo, document) for a service record.
/// Files are stored outside wwwroot and served through authorized endpoints.
/// </summary>
public class ServiceRecordAttachment : TenantEntity
{
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Whether this attachment should be visible in the public portal.
    /// </summary>
    public bool IsPublic { get; set; }

    // Foreign keys
    public Guid ServiceRecordId { get; set; }

    // Navigation properties
    public ServiceRecord ServiceRecord { get; set; } = null!;
}
