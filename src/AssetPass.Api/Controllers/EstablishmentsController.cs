using AssetPass.Application.Common;
using AssetPass.Application.Features.Establishments.Commands.CreateEstablishment;
using AssetPass.Application.Features.Establishments.Commands.DeleteEstablishment;
using AssetPass.Application.Features.Establishments.Commands.UpdateEstablishment;
using AssetPass.Application.Features.Establishments.Queries.GetEstablishmentById;
using AssetPass.Application.Features.Establishments.Queries.GetEstablishments;
using AssetPass.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// Manages client establishments/branches within the authenticated tenant.
/// Multi-tenancy isolation is strictly enforced via ITenantContext.
/// </summary>
[ApiController]
[Route("api/establishments")]
[Authorize]
public sealed class EstablishmentsController : ControllerBase
{
    private readonly GetEstablishmentsQueryHandler _getEstablishmentsHandler;
    private readonly GetEstablishmentByIdQueryHandler _getEstablishmentByIdHandler;
    private readonly CreateEstablishmentCommandHandler _createEstablishmentHandler;
    private readonly UpdateEstablishmentCommandHandler _updateEstablishmentHandler;
    private readonly DeleteEstablishmentCommandHandler _deleteEstablishmentHandler;
    private readonly ITenantContext _tenantContext;

    public EstablishmentsController(
        GetEstablishmentsQueryHandler getEstablishmentsHandler,
        GetEstablishmentByIdQueryHandler getEstablishmentByIdHandler,
        CreateEstablishmentCommandHandler createEstablishmentHandler,
        UpdateEstablishmentCommandHandler updateEstablishmentHandler,
        DeleteEstablishmentCommandHandler deleteEstablishmentHandler,
        ITenantContext tenantContext)
    {
        _getEstablishmentsHandler = getEstablishmentsHandler;
        _getEstablishmentByIdHandler = getEstablishmentByIdHandler;
        _createEstablishmentHandler = createEstablishmentHandler;
        _updateEstablishmentHandler = updateEstablishmentHandler;
        _deleteEstablishmentHandler = deleteEstablishmentHandler;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Returns a paginated list of establishments for the authenticated tenant with optional filters (e.g. by ClientId or search term).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<EstablishmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEstablishments([FromQuery] GetEstablishmentsQuery query, CancellationToken ct)
    {
        var result = await _getEstablishmentsHandler.HandleAsync(query, _tenantContext.TenantId, ct);
        return Ok(ApiResponse.Ok(result));
    }

    /// <summary>
    /// Returns detailed establishment information including equipment count.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EstablishmentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEstablishmentById(Guid id, CancellationToken ct)
    {
        var result = await _getEstablishmentByIdHandler.HandleAsync(id, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return NotFound(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok(result.Data!));
    }

    /// <summary>
    /// Creates a new establishment under an existing client in the tenant.
    /// Accessible by Owner, Administrator, Technician.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner,Administrator,Technician")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEstablishment([FromBody] CreateEstablishmentCommand command, CancellationToken ct)
    {
        var result = await _createEstablishmentHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
        {
            if (result.Error == "Cliente no encontrado en la empresa.")
                return NotFound(ApiResponse.Fail(result.Error));

            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(result.Data));
    }

    /// <summary>
    /// Updates establishment information.
    /// Accessible by Owner, Administrator, Technician.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Owner,Administrator,Technician")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEstablishment(
        Guid id,
        [FromBody] UpdateEstablishmentRequest request,
        CancellationToken ct)
    {
        var command = new UpdateEstablishmentCommand
        {
            Id = id,
            ClientId = request.ClientId,
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            ContactPerson = request.ContactPerson,
            Notes = request.Notes,
            IsActive = request.IsActive
        };

        var result = await _updateEstablishmentHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
        {
            if (result.Error == "Establecimiento no encontrado." || result.Error == "El cliente especificado no existe en la empresa.")
                return NotFound(ApiResponse.Fail(result.Error));

            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));
        }

        return Ok(ApiResponse.Ok("Establecimiento actualizado exitosamente."));
    }

    /// <summary>
    /// Soft deletes an establishment (marks IsActive as false).
    /// Accessible by Owner, Administrator.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEstablishment(Guid id, CancellationToken ct)
    {
        var result = await _deleteEstablishmentHandler.HandleAsync(id, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return NotFound(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok("Establecimiento eliminado exitosamente."));
    }
}
