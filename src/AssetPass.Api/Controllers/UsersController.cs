using AssetPass.Application.Common;
using AssetPass.Application.Features.Users.Commands.ChangeUserRole;
using AssetPass.Application.Features.Users.Queries.GetTenantUsers;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// User management within a tenant.
/// All endpoints require authentication. TenantId comes from JWT claims only.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly GetTenantUsersQueryHandler _getUsersHandler;
    private readonly ChangeUserRoleCommandHandler _changeRoleHandler;
    private readonly ITenantContext _tenantContext;

    public UsersController(
        GetTenantUsersQueryHandler getUsersHandler,
        ChangeUserRoleCommandHandler changeRoleHandler,
        ITenantContext tenantContext)
    {
        _getUsersHandler = getUsersHandler;
        _changeRoleHandler = changeRoleHandler;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Returns a paginated list of users in the authenticated tenant.
    /// Accessible by Owner and Administrator.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<TenantUserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetTenantUsersQuery query,
        CancellationToken ct)
    {
        var tenantId = _tenantContext.TenantId;
        var result = await _getUsersHandler.HandleAsync(query, tenantId, ct);
        return Ok(ApiResponse.Ok(result));
    }

    /// <summary>
    /// Changes the role of a user within the authenticated tenant.
    /// Owner: can change any role.
    /// Administrator: can change Technician/Viewer roles only, cannot touch Owner.
    /// </summary>
    [HttpPatch("{userId:guid}/role")]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(
        Guid userId,
        [FromBody] ChangeRoleRequest request,
        CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var newRole))
            return BadRequest(ApiResponse.Fail($"Rol inválido. Roles válidos: {string.Join(", ", Enum.GetNames<UserRole>())}"));

        var command = new ChangeUserRoleCommand
        {
            TargetUserId = userId,
            NewRole = newRole
        };

        if (!Enum.TryParse<UserRole>(_tenantContext.UserRole, ignoreCase: true, out var callerRole))
            return Forbid();

        var result = await _changeRoleHandler.HandleAsync(
            command,
            callerTenantId: _tenantContext.TenantId,
            callerApplicationUserId: _tenantContext.UserId,
            callerRole: callerRole,
            ct: ct);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "NOT_FOUND" => NotFound(ApiResponse.Fail(result.Error!)),
                "FORBIDDEN" => StatusCode(StatusCodes.Status403Forbidden, ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!))
            };
        }

        return Ok(ApiResponse.Ok($"Rol actualizado a '{newRole}' exitosamente."));
    }
}

public sealed class ChangeRoleRequest
{
    /// <summary>Role name: Owner, Administrator, Technician, Viewer</summary>
    public string Role { get; init; } = string.Empty;
}

