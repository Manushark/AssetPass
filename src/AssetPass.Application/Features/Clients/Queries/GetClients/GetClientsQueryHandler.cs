using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Clients.Queries.GetClients;

public sealed class GetClientsQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetClientsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedResult<ClientDto>> HandleAsync(
        GetClientsQuery query,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);

        var dbQuery = _db.Clients
            .AsNoTracking()
            .Where(c => c.TenantId == tenantId);

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(c => c.IsActive == query.IsActive.Value);

        if (query.ClientType.HasValue)
            dbQuery = dbQuery.Where(c => c.ClientType == query.ClientType.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            dbQuery = dbQuery.Where(c =>
                c.Name.ToLower().Contains(search) ||
                (c.Email != null && c.Email.ToLower().Contains(search)) ||
                (c.Phone != null && c.Phone.Contains(search)));
        }

        var totalCount = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ClientType = c.ClientType.ToString(),
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(ct);

        return new PaginatedResult<ClientDto>(items, totalCount, page, pageSize);
    }
}
