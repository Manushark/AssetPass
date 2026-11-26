using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Establishments.Commands.CreateEstablishment;

public sealed class CreateEstablishmentCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<CreateEstablishmentCommand> _validator;

    public CreateEstablishmentCommandHandler(
        IApplicationDbContext db,
        IValidator<CreateEstablishmentCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateEstablishmentCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result<Guid>.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        // Validate that the client exists and belongs to the current tenant
        var clientExists = await _db.Clients
            .AnyAsync(c => c.Id == command.ClientId && c.TenantId == tenantId, ct);

        if (!clientExists)
            return Result<Guid>.NotFound("Cliente no encontrado en la empresa.");

        var establishment = new Establishment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ClientId = command.ClientId,
            Name = command.Name.Trim(),
            Address = command.Address?.Trim(),
            Phone = command.Phone?.Trim(),
            ContactPerson = command.ContactPerson?.Trim(),
            Notes = command.Notes?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Establishments.Add(establishment);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(establishment.Id);
    }
}
