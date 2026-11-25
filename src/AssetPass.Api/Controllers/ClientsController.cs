using AssetPass.Application.Common;
using AssetPass.Application.Features.Clients.Commands.CreateClient;
using AssetPass.Application.Features.Clients.Commands.DeleteClient;
using AssetPass.Application.Features.Clients.Commands.UpdateClient;
using AssetPass.Application.Features.Clients.Queries.GetClientById;
using AssetPass.Application.Features.Clients.Queries.GetClients;
using AssetPass.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// Manages client accounts within the authenticated tenant.
/// Multi-tenancy isolation is strictly enforced via ITenantContext.
/// </summary>
[ApiController]
[Route("api/clients")]
[Authorize]
public sealed class ClientsController : ControllerBase
{
    private readonly GetClientsQueryHandler _getClientsHandler;
    private readonly GetClientByIdQueryHandler _getClientByIdHandler;
    private readonly CreateClientCommandHandler _createClientHandler;
    private readonly UpdateClientCommandHandler _updateClientHandler;
    private readonly DeleteClientCommandHandler _deleteClientHandler;
    private readonly ITenantContext _tenantContext;

    public ClientsController(
        GetClientsQueryHandler getClientsHandler,
        GetClientByIdQueryHandler getClientByIdHandler,
        CreateClientCommandHandler createClientHandler,
        UpdateClientCommandHandler updateClientHandler,
        DeleteClientCommandHandler deleteClientHandler,
        ITenantContext tenantContext)
    {
        _getClientsHandler = getClientsHandler;
        _getClientByIdHandler = getClientByIdHandler;
        _createClientHandler = createClientHandler;
        _updateClientHandler = updateClientHandler;
        _deleteClientHandler = deleteClientHandler;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Returns a paginated list of clients for the authenticated tenant with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ClientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClients([FromQuery] GetClientsQuery query, CancellationToken ct)
    {
        var result = await _getClientsHandler.HandleAsync(query, _tenantContext.TenantId, ct);
        return Ok(ApiResponse.Ok(result));
    }

    /// <summary>
    /// Returns detailed client profile including establishment and equipment counts.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClientDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientById(Guid id, CancellationToken ct)
    {
        var result = await _getClientByIdHandler.HandleAsync(id, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return NotFound(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok(result.Data!));
    }

    /// <summary>
    /// Creates a new client in the tenant.
    /// Accessible by Owner, Administrator, Technician.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner,Administrator,Technician")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command, CancellationToken ct)
    {
        var result = await _createClientHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(result.Data));
    }

    /// <summary>
    /// Updates client information.
    /// Accessible by Owner, Administrator, Technician.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Owner,Administrator,Technician")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClient(
        Guid id,
        [FromBody] UpdateClientRequest request,
        CancellationToken ct)
    {
        var command = new UpdateClientCommand
        {
            ClientId = id,
            Name = request.Name,
            ClientType = request.ClientType,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Notes = request.Notes,
            IsActive = request.IsActive
        };

        var result = await _updateClientHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "NOT_FOUND" => NotFound(ApiResponse.Fail(result.Error!)),
                _ => BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors))
            };
        }

        return Ok(ApiResponse.Ok("Cliente actualizado exitosamente."));
    }

    /// <summary>
    /// Soft deletes (deactivates) a client.
    /// Accessible by Owner and Administrator.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(Guid id, CancellationToken ct)
    {
        var result = await _deleteClientHandler.HandleAsync(id, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return NotFound(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok("Cliente desactivado exitosamente."));
    }
}

public sealed class UpdateClientRequest
{
    public string Name { get; init; } = string.Empty;
    public Domain.Enums.ClientType ClientType { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}
