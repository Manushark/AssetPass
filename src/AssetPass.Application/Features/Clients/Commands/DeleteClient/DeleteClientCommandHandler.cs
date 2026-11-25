using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Clients.Commands.DeleteClient;

public sealed class DeleteClientCommandHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteClientCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> HandleAsync(
        Guid clientId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var client = await _db.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId && c.TenantId == tenantId, ct);

        if (client is null)
            return Result.NotFound("Cliente no encontrado.");

        // Soft delete for auditability and preservation of historical passports
        client.IsActive = false;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
