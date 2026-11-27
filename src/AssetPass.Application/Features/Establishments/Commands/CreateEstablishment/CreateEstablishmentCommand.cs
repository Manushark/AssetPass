namespace AssetPass.Application.Features.Establishments.Commands.CreateEstablishment;

public sealed class CreateEstablishmentCommand
{
    public Guid ClientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? ContactPerson { get; init; }
    public string? Notes { get; init; }
}
