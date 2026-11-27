using System.Text;
using AssetPass.Api.Controllers;
using AssetPass.Api.Middleware;
using AssetPass.Api.Services;
using AssetPass.Application.Features.Auth.Commands.AcceptInvitation;
using AssetPass.Application.Features.Auth.Commands.ChangePassword;
using AssetPass.Application.Features.Auth.Commands.ForgotPassword;
using AssetPass.Application.Features.Auth.Commands.Login;
using AssetPass.Application.Features.Auth.Commands.Logout;
using AssetPass.Application.Features.Auth.Commands.RefreshToken;
using AssetPass.Application.Features.Auth.Commands.RegisterTenant;
using AssetPass.Application.Features.Auth.Commands.ResetPassword;
using AssetPass.Application.Features.Clients.Commands.CreateClient;
using AssetPass.Application.Features.Clients.Commands.DeleteClient;
using AssetPass.Application.Features.Clients.Commands.UpdateClient;
using AssetPass.Application.Features.Clients.Queries.GetClientById;
using AssetPass.Application.Features.Clients.Queries.GetClients;
using AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;
using AssetPass.Application.Features.Company.Commands.UpdatePublicPortalSettings;
using AssetPass.Application.Features.Company.Queries.GetCompanyProfile;
using AssetPass.Application.Features.Users.Commands.ChangeUserRole;
using AssetPass.Application.Features.Users.Commands.InviteUser;
using AssetPass.Application.Features.Users.Queries.GetTenantUsers;
using AssetPass.Application.Interfaces;
using AssetPass.Infrastructure;
using AssetPass.Infrastructure.Persistence;
using AspNetCoreRateLimit;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SERVICES
// ============================================================

// Infrastructure (Database, Identity, File Storage, Email, JWT, Refresh Tokens)
builder.Services.AddInfrastructure(builder.Configuration);

// Tenant Context (derived from JWT claims — TenantId never comes from the client body)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();

// JWT Authentication middleware
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured. Set Jwt:Key in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// FluentValidation validators (registered from Application assembly)
builder.Services.AddValidatorsFromAssemblyContaining<RegisterTenantCommandValidator>();

// Application command handlers (direct injection, no MediatR)
builder.Services.AddScoped<RegisterTenantCommandHandler>();
builder.Services.AddScoped<LoginCommandHandler>();
builder.Services.AddScoped<RefreshTokenCommandHandler>();
builder.Services.AddScoped<LogoutCommandHandler>();
builder.Services.AddScoped<ForgotPasswordCommandHandler>();
builder.Services.AddScoped<ResetPasswordCommandHandler>();
builder.Services.AddScoped<ChangePasswordCommandHandler>();
builder.Services.AddScoped<GetTenantUsersQueryHandler>();
builder.Services.AddScoped<ChangeUserRoleCommandHandler>();
builder.Services.AddScoped<InviteUserCommandHandler>();
builder.Services.AddScoped<AcceptInvitationCommandHandler>();
builder.Services.AddScoped<GetCompanyProfileQueryHandler>();
builder.Services.AddScoped<UpdateCompanyProfileCommandHandler>();
builder.Services.AddScoped<UpdatePublicPortalSettingsCommandHandler>();
builder.Services.AddScoped<GetClientsQueryHandler>();
builder.Services.AddScoped<GetClientByIdQueryHandler>();
builder.Services.AddScoped<CreateClientCommandHandler>();
builder.Services.AddScoped<UpdateClientCommandHandler>();
builder.Services.AddScoped<DeleteClientCommandHandler>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200" };
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimit"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AssetPass API",
        Version = "v1",
        Description = "Digital passport platform for equipment management. Multi-tenant SaaS API."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ============================================================
// PIPELINE
// ============================================================
var app = builder.Build();

// Exception handling (first in pipeline)
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AssetPass API v1");
        options.RoutePrefix = "swagger";
    });
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseIpRateLimiting();
app.UseCors("AllowAngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed roles gracefully if DB is available
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
    if (dbContext != null && dbContext.Database.CanConnect())
    {
        await RoleSeeder.SeedRolesAsync(app.Services);
    }
}
catch
{
    // DB might be offline during initial build or tests
}

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
