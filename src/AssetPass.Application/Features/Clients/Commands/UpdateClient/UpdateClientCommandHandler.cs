using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Clients.Commands.UpdateClient;

public sealed class UpdateClientCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<UpdateClientCommand> _validator;

    public UpdateClientCommandHandler(
        IApplicationDbContext db,
        IValidator<UpdateClientCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(
        UpdateClientCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        var client = await _db.Clients
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && c.TenantId == tenantId, ct);

        if (client is null)
            return Result.NotFound("Cliente no encontrado.");

        client.Name = command.Name.Trim();
        client.ClientType = command.ClientType;
        client.Phone = command.Phone?.Trim();
        client.Email = command.Email?.Trim().ToLowerInvariant();
        client.Address = command.Address?.Trim();
        client.Notes = command.Notes?.Trim();
        client.IsActive = command.IsActive;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
