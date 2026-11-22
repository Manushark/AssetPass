using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Users.Commands.ChangeUserRole;

/// <summary>
/// Handles role changes within a tenant.
///
/// Authorization rules enforced here (in addition to [Authorize] on the controller):
/// - Only Owner or Administrator can change roles.
/// - An Administrator cannot assign or modify the Owner role.
/// - A user cannot change their own role.
/// - TenantId is always validated from the JWT claim — never from the request body.
/// </summary>
public sealed class ChangeUserRoleCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public ChangeUserRoleCommandHandler(
        IApplicationDbContext db,
        IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<Result> HandleAsync(
        ChangeUserRoleCommand command,
        Guid callerTenantId,
        Guid callerApplicationUserId,
        UserRole callerRole,
        CancellationToken ct = default)
    {
        // 1. Prevent self-role-change
        if (command.TargetUserId == callerApplicationUserId)
            return Result.Failure("No puedes cambiar tu propio rol.");

        // 2. Only Owner or Administrator can invoke this
        if (callerRole != UserRole.Owner && callerRole != UserRole.Administrator)
            return Result.Forbidden("No tienes permiso para cambiar roles de usuario.");

        // 3. Administrator cannot assign or demote Owner
        if (callerRole == UserRole.Administrator &&
            (command.NewRole == UserRole.Owner))
            return Result.Forbidden("Un Administrador no puede asignar el rol Owner.");

        // 4. Load target user — must belong to same tenant
        var targetUser = await _db.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == command.TargetUserId && u.TenantId == callerTenantId, ct);

        if (targetUser is null)
            return Result.NotFound("Usuario no encontrado en este tenant.");

        // 5. Administrator cannot modify the Owner
        if (callerRole == UserRole.Administrator && targetUser.Role == UserRole.Owner)
            return Result.Forbidden("Un Administrador no puede modificar el rol del Owner.");

        var oldRole = targetUser.Role;

        // 6. Update domain entity
        targetUser.Role = command.NewRole;

        // 7. Update Identity role
        var identityUser = await _identityService.FindByEmailAsync(targetUser.IdentityUserId, ct);
        if (identityUser is not null)
        {
            await _identityService.ChangeUserRoleAsync(
                targetUser.IdentityUserId,
                oldRole.ToString(),
                command.NewRole.ToString(),
                ct);
        }

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}

