namespace AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;

public sealed class UpdateCompanyProfileCommand
{
    public string CommercialName { get; init; } = string.Empty;
    public string? LegalName { get; init; }
    public string? Rnc { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string Currency { get; init; } = "DOP";
    public string TimeZone { get; init; } = "America/Santo_Domingo";
    public string Locale { get; init; } = "es-DO";
}
