using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Clients.Queries.GetClientById;

public sealed class GetClientByIdQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetClientByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ClientDetailDto>> HandleAsync(
        Guid clientId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var client = await _db.Clients
            .AsNoTracking()
            .Where(c => c.Id == clientId && c.TenantId == tenantId)
            .Select(c => new ClientDetailDto
            {
                Id = c.Id,
                Name = c.Name,
                ClientType = c.ClientType.ToString(),
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                Notes = c.Notes,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                EstablishmentsCount = c.Establishments.Count,
                EquipmentCount = c.Equipment.Count
            })
            .FirstOrDefaultAsync(ct);

        if (client is null)
            return Result<ClientDetailDto>.NotFound("Cliente no encontrado.");

        return Result<ClientDetailDto>.Success(client);
    }
}
