using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Establishments.Queries.GetEstablishmentById;

public sealed class GetEstablishmentByIdQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetEstablishmentByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EstablishmentDetailDto>> HandleAsync(
        Guid establishmentId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var establishment = await _db.Establishments
            .AsNoTracking()
            .Include(e => e.Client)
            .Where(e => e.Id == establishmentId && e.TenantId == tenantId)
            .Select(e => new EstablishmentDetailDto
            {
                Id = e.Id,
                ClientId = e.ClientId,
                ClientName = e.Client.Name,
                Name = e.Name,
                Address = e.Address,
                Phone = e.Phone,
                ContactPerson = e.ContactPerson,
                Notes = e.Notes,
                IsActive = e.IsActive,
                EquipmentCount = e.Equipment.Count,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (establishment is null)
            return Result<EstablishmentDetailDto>.NotFound("Establecimiento no encontrado.");

        return Result<EstablishmentDetailDto>.Success(establishment);
    }
}
