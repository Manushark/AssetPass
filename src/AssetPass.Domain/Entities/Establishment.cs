using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a physical location (branch/site) of a client.
/// A client can have multiple establishments, each containing equipment.
/// </summary>
public class Establishment : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public Guid ClientId { get; set; }

    // Navigation properties
    public Client Client { get; set; } = null!;
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
