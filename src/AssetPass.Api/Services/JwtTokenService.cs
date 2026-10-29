using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetPass.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AssetPass.Api.Services;

/// <summary>
/// JWT token generation service.
/// Creates access tokens with tenant and user claims.
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppIdentityUser> _userManager;

    public JwtTokenService(IConfiguration configuration, UserManager<AppIdentityUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    public string GenerateAccessToken(AppIdentityUser identityUser, string role, Guid applicationUserId)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityUser.Id),
            new(JwtRegisteredClaimNames.Email, identityUser.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant_id", identityUser.TenantId.ToString()),
            new("app_user_id", applicationUserId.ToString()),
            new(ClaimTypes.Email, identityUser.Email ?? string.Empty),
            new(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationMinutes = int.TryParse(jwtSettings["ExpirationMinutes"], out var minutes) ? minutes : 60;

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
