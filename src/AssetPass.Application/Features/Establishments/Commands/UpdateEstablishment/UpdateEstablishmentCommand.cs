namespace AssetPass.Application.Features.Establishments.Commands.UpdateEstablishment;

public sealed class UpdateEstablishmentRequest
{
    public Guid ClientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? ContactPerson { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class UpdateEstablishmentCommand
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? ContactPerson { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}
