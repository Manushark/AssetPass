using AssetPass.Domain.Enums;

namespace AssetPass.Application.Features.Clients.Commands.UpdateClient;

public sealed class UpdateClientCommand
{
    public Guid ClientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public ClientType ClientType { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}
