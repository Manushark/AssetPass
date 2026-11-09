using AssetPass.Domain.Enums;

namespace AssetPass.Application.Features.Users.Commands.ChangeUserRole;

/// <summary>
/// Changes the role of a user within the authenticated tenant.
/// Only Owner and Administrator can change roles.
/// An Owner cannot have their role changed by an Administrator.
/// </summary>
public sealed class ChangeUserRoleCommand
{
    /// <summary>The domain ApplicationUser ID of the user whose role changes.</summary>
    public Guid TargetUserId { get; init; }

    public UserRole NewRole { get; init; }
}

