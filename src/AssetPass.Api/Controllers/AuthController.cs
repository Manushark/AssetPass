using System.Security.Claims;
using AssetPass.Application.Common;
using AssetPass.Application.Features.Auth.Commands.AcceptInvitation;
using AssetPass.Application.Features.Auth.Commands.ChangePassword;
using AssetPass.Application.Features.Auth.Commands.ForgotPassword;
using AssetPass.Application.Features.Auth.Commands.Login;
using AssetPass.Application.Features.Auth.Commands.Logout;
using AssetPass.Application.Features.Auth.Commands.RefreshToken;
using AssetPass.Application.Features.Auth.Commands.RegisterTenant;
using AssetPass.Application.Features.Auth.Commands.ResetPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetPass.Api.Controllers;

/// <summary>
/// Handles authentication, company onboarding, tokens, and password management.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterTenantCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;
    private readonly RefreshTokenCommandHandler _refreshHandler;
    private readonly LogoutCommandHandler _logoutHandler;
    private readonly ForgotPasswordCommandHandler _forgotPasswordHandler;
    private readonly ResetPasswordCommandHandler _resetPasswordHandler;
    private readonly ChangePasswordCommandHandler _changePasswordHandler;
    private readonly AcceptInvitationCommandHandler _acceptInvitationHandler;

    public AuthController(
        RegisterTenantCommandHandler registerHandler,
        LoginCommandHandler loginHandler,
        RefreshTokenCommandHandler refreshHandler,
        LogoutCommandHandler logoutHandler,
        ForgotPasswordCommandHandler forgotPasswordHandler,
        ResetPasswordCommandHandler resetPasswordHandler,
        ChangePasswordCommandHandler changePasswordHandler,
        AcceptInvitationCommandHandler acceptInvitationHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resetPasswordHandler = resetPasswordHandler;
        _changePasswordHandler = changePasswordHandler;
        _acceptInvitationHandler = acceptInvitationHandler;
    }

    /// <summary>
    /// Registers a new company (tenant) and its first admin user (Owner role).
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken ct)
    {
        var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        var result = await _logoutHandler.HandleAsync(command, identityUserId, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!));

        return Ok(ApiResponse.Ok("Sesión cerrada exitosamente."));
    }

    /// <summary>
    /// Requests a password reset link/token. Always returns success to prevent user enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken ct)
    {
        var result = await _forgotPasswordHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return Ok(ApiResponse.Ok("Si el correo electrónico existe en nuestra plataforma, recibirás instrucciones para restablecer tu contraseña."));
    }

    /// <summary>
    /// Resets user password using the token sent via email.
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken ct)
    {
        var result = await _resetPasswordHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return Ok(ApiResponse.Ok("Contraseña restablecida exitosamente. Ya puedes iniciar sesión con tu nueva contraseña."));
    }

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken ct)
    {
        var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(identityUserId))
            return Unauthorized(ApiResponse.Fail("Usuario no autenticado."));

        var result = await _changePasswordHandler.HandleAsync(command, identityUserId, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return Ok(ApiResponse.Ok("Contraseña modificada exitosamente. Todas las sesiones han sido cerradas por seguridad."));
    }

    /// <summary>
    /// Accepts a user invitation using a secure token and sets up user credentials.
    /// </summary>
    [HttpPost("accept-invitation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptInvitation(
        [FromBody] AcceptInvitationCommand command,
        CancellationToken ct)
    {
        var result = await _acceptInvitationHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Fail(result.Error!, result.ValidationErrors));

        return Ok(ApiResponse.Ok("Invitación aceptada exitosamente. Ya puedes iniciar sesión con tu cuenta."));
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
