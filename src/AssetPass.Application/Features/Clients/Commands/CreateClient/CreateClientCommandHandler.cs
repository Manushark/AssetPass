using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Entities;
using FluentValidation;

namespace AssetPass.Application.Features.Clients.Commands.CreateClient;

public sealed class CreateClientCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<CreateClientCommand> _validator;

    public CreateClientCommandHandler(
        IApplicationDbContext db,
        IValidator<CreateClientCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateClientCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result<Guid>.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        var client = new Client
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = command.Name.Trim(),
            ClientType = command.ClientType,
            Phone = command.Phone?.Trim(),
            Email = command.Email?.Trim().ToLowerInvariant(),
            Address = command.Address?.Trim(),
            Notes = command.Notes?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(client.Id);
    }
}
