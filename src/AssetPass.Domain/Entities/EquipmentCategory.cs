using AssetPass.Domain.Common;

namespace AssetPass.Domain.Entities;

/// <summary>
/// Configurable equipment category. Allows tenants to define
/// their own categories beyond the default ones.
/// </summary>
public class EquipmentCategory : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
