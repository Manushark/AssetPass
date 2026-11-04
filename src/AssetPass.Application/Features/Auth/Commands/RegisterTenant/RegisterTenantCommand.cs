namespace AssetPass.Application.Features.Auth.Commands.RegisterTenant;

/// <summary>
/// Command to register a new company (tenant) and its initial admin user.
/// This is the primary entry point for onboarding new companies into AssetPass.
/// </summary>
public sealed class RegisterTenantCommand
{
    // Company information
    public string CommercialName { get; init; } = string.Empty;
    public string? LegalName { get; init; }
    public string? Rnc { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string CompanyEmail { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string Currency { get; init; } = "DOP";
    public string TimeZone { get; init; } = "America/Santo_Domingo";

    // Initial admin user
    public string AdminFirstName { get; init; } = string.Empty;
    public string AdminLastName { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;
    public string AdminPasswordConfirm { get; init; } = string.Empty;
}

