namespace AssetPass.Application.Features.Company.Queries.GetCompanyProfile;

public sealed class CompanyProfileDto
{
    public Guid Id { get; init; }
    public string CommercialName { get; init; } = string.Empty;
    public string? LegalName { get; init; }
    public string? Rnc { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? LogoPath { get; init; }
    public string Currency { get; init; } = "DOP";
    public string TimeZone { get; init; } = "America/Santo_Domingo";
    public string Locale { get; init; } = "es-DO";
    public string SubscriptionStatus { get; init; } = string.Empty;
    public DateTime? TrialEndDate { get; init; }
    public bool IsActive { get; init; }

    // Public portal settings
    public bool ShowWarrantyInPublicPortal { get; init; }
    public bool ShowMaintenanceHistoryInPublicPortal { get; init; }
    public bool ShowNextMaintenanceInPublicPortal { get; init; }
    public bool ShowLastMaintenanceInPublicPortal { get; init; }
    public bool ShowEquipmentPhotoInPublicPortal { get; init; }
    public bool ShowCompanyInfoInPublicPortal { get; init; }
}

public sealed class GetCompanyProfileQuery
{
}
