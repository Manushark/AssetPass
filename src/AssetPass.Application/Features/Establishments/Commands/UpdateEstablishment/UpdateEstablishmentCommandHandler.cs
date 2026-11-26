using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Establishments.Commands.UpdateEstablishment;

public sealed class UpdateEstablishmentCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<UpdateEstablishmentCommand> _validator;

    public UpdateEstablishmentCommandHandler(
        IApplicationDbContext db,
        IValidator<UpdateEstablishmentCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(
        UpdateEstablishmentCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        var establishment = await _db.Establishments
            .FirstOrDefaultAsync(e => e.Id == command.Id && e.TenantId == tenantId, ct);

        if (establishment is null)
            return Result.NotFound("Establecimiento no encontrado.");

        // If client changed, verify the new client exists in the tenant
        if (establishment.ClientId != command.ClientId)
        {
            var clientExists = await _db.Clients
                .AnyAsync(c => c.Id == command.ClientId && c.TenantId == tenantId, ct);

            if (!clientExists)
                return Result.NotFound("El cliente especificado no existe en la empresa.");

            establishment.ClientId = command.ClientId;
        }

        establishment.Name = command.Name.Trim();
        establishment.Address = command.Address?.Trim();
        establishment.Phone = command.Phone?.Trim();
        establishment.ContactPerson = command.ContactPerson?.Trim();
        establishment.Notes = command.Notes?.Trim();
        establishment.IsActive = command.IsActive;
        establishment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
