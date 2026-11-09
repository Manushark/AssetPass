using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Users.Queries.GetTenantUsers;

/// <summary>
/// Returns a paginated list of all users in the authenticated tenant.
/// TenantId is always sourced from the JWT claim.
/// </summary>
public sealed class GetTenantUsersQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetTenantUsersQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedResult<TenantUserDto>> HandleAsync(
        GetTenantUsersQuery query,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var page = Math.Max(1, query.Page);

        var baseQuery = _db.ApplicationUsers
            .Where(u => u.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLower();
            baseQuery = baseQuery.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }

        var total = await baseQuery.CountAsync(ct);

        var users = await baseQuery
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new TenantUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PaginatedResult<TenantUserDto>(users, total, page, pageSize);
    }
}

