namespace AssetPass.Application.Features.Company.Commands.UpdatePublicPortalSettings;

public sealed class UpdatePublicPortalSettingsCommand
{
    public bool ShowWarrantyInPublicPortal { get; init; }
    public bool ShowMaintenanceHistoryInPublicPortal { get; init; }
    public bool ShowNextMaintenanceInPublicPortal { get; init; }
    public bool ShowLastMaintenanceInPublicPortal { get; init; }
    public bool ShowEquipmentPhotoInPublicPortal { get; init; }
    public bool ShowCompanyInfoInPublicPortal { get; init; }
}
