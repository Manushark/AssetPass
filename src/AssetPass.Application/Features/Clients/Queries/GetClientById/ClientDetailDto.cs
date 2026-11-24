namespace AssetPass.Application.Features.Clients.Queries.GetClientById;

public sealed class ClientDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ClientType { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int EstablishmentsCount { get; init; }
    public int EquipmentCount { get; init; }
}

public sealed class GetClientByIdQuery
{
    public Guid ClientId { get; init; }
}
