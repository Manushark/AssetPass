using AssetPass.Application.Common;
using AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;
using AssetPass.Application.Features.Company.Commands.UpdatePublicPortalSettings;
using AssetPass.Application.Features.Company.Queries.GetCompanyProfile;
using AssetPass.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// Manages tenant company profile and configuration settings.
/// TenantId is derived strictly from JWT claims.
/// </summary>
[ApiController]
[Route("api/company")]
[Authorize]
public sealed class CompanyController : ControllerBase
{
    private readonly GetCompanyProfileQueryHandler _getProfileHandler;
    private readonly UpdateCompanyProfileCommandHandler _updateProfileHandler;
    private readonly UpdatePublicPortalSettingsCommandHandler _updatePortalHandler;
    private readonly ITenantContext _tenantContext;

    public CompanyController(
        GetCompanyProfileQueryHandler getProfileHandler,
        UpdateCompanyProfileCommandHandler updateProfileHandler,
        UpdatePublicPortalSettingsCommandHandler updatePortalHandler,
        ITenantContext tenantContext)
    {
        _getProfileHandler = getProfileHandler;
        _updateProfileHandler = updateProfileHandler;
        _updatePortalHandler = updatePortalHandler;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Returns the company profile for the authenticated tenant.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CompanyProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await _getProfileHandler.HandleAsync(_tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return NotFound(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok(result.Data!));
    }

    /// <summary>
    /// Updates commercial and administrative settings for the authenticated tenant.
    /// Accessible by Owner and Administrator.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateCompanyProfileCommand command,
        CancellationToken ct)
    {
        var result = await _updateProfileHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return Ok(ApiResponse.Ok("Perfil de empresa actualizado exitosamente."));
    }

    /// <summary>
    /// Updates public portal visibility settings for equipment passports.
    /// Accessible by Owner and Administrator.
    /// </summary>
    [HttpPatch("public-portal")]
    [Authorize(Roles = "Owner,Administrator")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePublicPortalSettings(
        [FromBody] UpdatePublicPortalSettingsCommand command,
        CancellationToken ct)
    {
        var result = await _updatePortalHandler.HandleAsync(command, _tenantContext.TenantId, ct);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok("Configuración de portal público actualizada exitosamente."));
    }
}
