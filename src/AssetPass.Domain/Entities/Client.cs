using AssetPass.Domain.Common;
using AssetPass.Domain.Enums;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Represents a client of a tenant company.
/// A client can have multiple establishments and equipment.
/// </summary>
public class Client : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Establishment> Establishments { get; set; } = new List<Establishment>();
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
