using AssetPass.Application.Common;
using AssetPass.Application.Features.Auth.Commands.Login;
using AssetPass.Application.Features.Auth.Commands.Logout;
using AssetPass.Application.Features.Auth.Commands.RefreshToken;
using AssetPass.Application.Features.Auth.Commands.RegisterTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// Handles company registration, login, and token refresh.
/// All endpoints are anonymous — no [Authorize] required.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterTenantCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;
    private readonly RefreshTokenCommandHandler _refreshHandler;
    private readonly LogoutCommandHandler _logoutHandler;

    public AuthController(
        RegisterTenantCommandHandler registerHandler,
        LoginCommandHandler loginHandler,
        RefreshTokenCommandHandler refreshHandler,
        LogoutCommandHandler logoutHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
    }

    /// <summary>
    /// Registers a new company (tenant) and its first admin user (Owner role).
    /// </summary>
    /// <remarks>
    /// Creates a new isolated tenant context. The supplied admin credentials
    /// become the Owner of the new company. A 14-day trial subscription is started.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterTenantCommand command,
        CancellationToken ct)
    {
        var result = await _registerHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            var statusCode = result.ErrorCode == "CONFLICT"
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status400BadRequest;

            return StatusCode(statusCode, ApiResponse.Fail(result.Error!, result.ValidationErrors));
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(result.Data));
    }

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        var result = await _loginHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
            return Unauthorized(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok(result.Data!));
    }

    /// <summary>
    /// Exchanges a valid refresh token for new access and refresh tokens.
    /// The submitted refresh token is revoked immediately (rotation pattern).
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
    {
        var result = await _refreshHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
            return Unauthorized(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok(result.Data!));
    }

    /// <summary>
    /// Logs out a user and revokes their refresh token(s).
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken ct)
    {
        var identityUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        var result = await _logoutHandler.HandleAsync(command, identityUserId, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok("Sesión cerrada exitosamente."));
    }
}

/// <summary>
/// Envelope wrapper for all API responses, providing a consistent contract.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> ValidationErrors { get; init; } = [];
}

/// <summary>
/// Factory helpers for ApiResponse without needing to specify the type parameter.
/// </summary>
public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data) => new() { Success = true, Data = data };

    public static ApiResponse<object> Fail(string error, IReadOnlyList<string>? validationErrors = null) =>
        new() { Success = false, Error = error, ValidationErrors = validationErrors ?? [] };
}

