using AssetPass.Domain.Enums;

namespace AssetPass.Application.Features.Clients.Queries.GetClients;

public sealed class ClientDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ClientType { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class GetClientsQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public ClientType? ClientType { get; init; }
    public bool? IsActive { get; init; }
}
