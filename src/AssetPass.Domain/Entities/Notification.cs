using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a notification in the system.
/// Tracks sent notifications to prevent duplicates.
/// </summary>
public class Notification : TenantEntity
{
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Reference to the entity that triggered this notification.
    /// Used together with ReferenceType for navigation.
    /// </summary>
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }

    // Foreign keys
    public Guid RecipientUserId { get; set; }

    // Navigation properties
    public ApplicationUser Recipient { get; set; } = null!;

    /// <summary>
    /// Unique key to prevent sending the same notification twice.
    /// Format: "{Type}:{ReferenceId}:{Date}"
    /// </summary>
    public string? DeduplicationKey { get; set; }
}
