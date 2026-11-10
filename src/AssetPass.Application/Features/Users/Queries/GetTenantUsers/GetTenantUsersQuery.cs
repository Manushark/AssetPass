namespace AssetPass.Application.Features.Users.Queries.GetTenantUsers;

/// <summary>
/// Returns a paginated list of users belonging to the authenticated tenant.
/// </summary>
public sealed class GetTenantUsersQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}

public sealed class TenantUserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

