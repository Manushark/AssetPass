using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Establishments.Queries.GetEstablishments;

public sealed class GetEstablishmentsQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetEstablishmentsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedResult<EstablishmentDto>> HandleAsync(
        GetEstablishmentsQuery query,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);

        var dbQuery = _db.Establishments
            .AsNoTracking()
            .Include(e => e.Client)
            .Where(e => e.TenantId == tenantId);

        if (query.ClientId.HasValue)
            dbQuery = dbQuery.Where(e => e.ClientId == query.ClientId.Value);

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(e => e.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            dbQuery = dbQuery.Where(e =>
                e.Name.ToLower().Contains(search) ||
                (e.Address != null && e.Address.ToLower().Contains(search)) ||
                (e.ContactPerson != null && e.ContactPerson.ToLower().Contains(search)) ||
                e.Client.Name.ToLower().Contains(search));
        }

        var totalCount = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EstablishmentDto
            {
                Id = e.Id,
                ClientId = e.ClientId,
                ClientName = e.Client.Name,
                Name = e.Name,
                Address = e.Address,
                Phone = e.Phone,
                ContactPerson = e.ContactPerson,
                IsActive = e.IsActive,
                EquipmentCount = e.Equipment.Count,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(ct);

        return new PaginatedResult<EstablishmentDto>(items, totalCount, page, pageSize);
    }
}
