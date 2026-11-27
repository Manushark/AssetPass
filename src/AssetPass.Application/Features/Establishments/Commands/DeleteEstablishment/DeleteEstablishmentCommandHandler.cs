using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Establishments.Commands.DeleteEstablishment;

public sealed class DeleteEstablishmentCommandHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteEstablishmentCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> HandleAsync(
        Guid establishmentId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var establishment = await _db.Establishments
            .FirstOrDefaultAsync(e => e.Id == establishmentId && e.TenantId == tenantId, ct);

        if (establishment is null)
            return Result.NotFound("Establecimiento no encontrado.");

        // Soft delete
        establishment.IsActive = false;
        establishment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
