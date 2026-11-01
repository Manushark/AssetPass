using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetPass.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetPass.Infrastructure.Services;

/// <summary>
/// JWT token generation service implementing IJwtTokenService.
/// Moved from AssetPass.Api to Infrastructure so it can be injected into Application handlers.
/// The JwtTokenService in the Api project is now removed; this is the single source of truth.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string GenerateAccessToken(
        string identityUserId,
        string email,
        Guid tenantId,
        Guid applicationUserId,
        string role)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured.")));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityUserId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant_id", tenantId.ToString()),
            new("app_user_id", applicationUserId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = GetAccessTokenExpirationMinutes();

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc />
    public int GetAccessTokenExpirationMinutes()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        return int.TryParse(jwtSettings["ExpirationMinutes"], out var minutes) ? minutes : 60;
    }
}

