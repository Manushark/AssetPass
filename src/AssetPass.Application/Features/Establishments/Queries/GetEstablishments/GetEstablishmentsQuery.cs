namespace AssetPass.Application.Features.Establishments.Queries.GetEstablishments;

public sealed class EstablishmentDto
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? ContactPerson { get; init; }
    public bool IsActive { get; init; }
    public int EquipmentCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class GetEstablishmentsQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? ClientId { get; init; }
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
}
