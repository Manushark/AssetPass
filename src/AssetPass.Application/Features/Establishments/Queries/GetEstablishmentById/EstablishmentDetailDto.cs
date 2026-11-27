namespace AssetPass.Application.Features.Establishments.Queries.GetEstablishmentById;

public sealed class EstablishmentDetailDto
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? ContactPerson { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public int EquipmentCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class GetEstablishmentByIdQuery
{
    public Guid EstablishmentId { get; init; }
}
