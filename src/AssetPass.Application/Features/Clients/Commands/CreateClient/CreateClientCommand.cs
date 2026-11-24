using AssetPass.Domain.Enums;

namespace AssetPass.Application.Features.Clients.Commands.CreateClient;

public sealed class CreateClientCommand
{
    public string Name { get; init; } = string.Empty;
    public ClientType ClientType { get; init; } = ClientType.Business;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
}
