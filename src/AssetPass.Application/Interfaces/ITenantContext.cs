namespace AssetPass.Application.Interfaces;

/// <summary>
/// Provides the current tenant context for the authenticated user.
/// TenantId is derived from the server-side JWT claims, never from client input.
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }
    Guid UserId { get; }
    string UserEmail { get; }
    string UserRole { get; }
    bool IsAuthenticated { get; }
}
