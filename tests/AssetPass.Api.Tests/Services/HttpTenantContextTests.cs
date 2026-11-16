using System.Security.Claims;
using AssetPass.Api.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AssetPass.Api.Tests.Services;

public class HttpTenantContextTests
{
    private class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    [Fact]
    public void Properties_WithValidClaims_ReturnsExpectedValues()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var role = "Administrator";

        var claims = new[]
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("app_user_id", userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        var accessor = new FakeHttpContextAccessor { HttpContext = httpContext };

        var context = new HttpTenantContext(accessor);

        Assert.True(context.IsAuthenticated);
        Assert.Equal(tenantId, context.TenantId);
        Assert.Equal(userId, context.UserId);
        Assert.Equal(email, context.UserEmail);
        Assert.Equal(role, context.UserRole);
    }

    [Fact]
    public void Properties_WhenUnauthenticated_ReturnsEmptyDefaults()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = new FakeHttpContextAccessor { HttpContext = httpContext };

        var context = new HttpTenantContext(accessor);

        Assert.False(context.IsAuthenticated);
        Assert.Equal(Guid.Empty, context.TenantId);
        Assert.Equal(Guid.Empty, context.UserId);
        Assert.Equal(string.Empty, context.UserEmail);
        Assert.Equal(string.Empty, context.UserRole);
    }

    [Fact]
    public void Properties_WhenHttpContextIsNull_ReturnsEmptyDefaults()
    {
        var accessor = new FakeHttpContextAccessor { HttpContext = null };

        var context = new HttpTenantContext(accessor);

        Assert.False(context.IsAuthenticated);
        Assert.Equal(Guid.Empty, context.TenantId);
        Assert.Equal(Guid.Empty, context.UserId);
        Assert.Equal(string.Empty, context.UserEmail);
        Assert.Equal(string.Empty, context.UserRole);
    }

    [Fact]
    public void TenantId_WithInvalidGuidClaim_ReturnsGuidEmpty()
    {
        var claims = new[] { new Claim("tenant_id", "not-a-valid-guid") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        var accessor = new FakeHttpContextAccessor { HttpContext = httpContext };

        var context = new HttpTenantContext(accessor);

        Assert.Equal(Guid.Empty, context.TenantId);
    }
}
