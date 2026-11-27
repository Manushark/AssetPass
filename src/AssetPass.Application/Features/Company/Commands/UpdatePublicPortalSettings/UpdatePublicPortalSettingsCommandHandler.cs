using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Company.Commands.UpdatePublicPortalSettings;

public sealed class UpdatePublicPortalSettingsCommandHandler
{
    private readonly IApplicationDbContext _db;

    public UpdatePublicPortalSettingsCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> HandleAsync(
        UpdatePublicPortalSettingsCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null)
            return Result.NotFound("Empresa no encontrada.");

        tenant.ShowWarrantyInPublicPortal = command.ShowWarrantyInPublicPortal;
        tenant.ShowMaintenanceHistoryInPublicPortal = command.ShowMaintenanceHistoryInPublicPortal;
        tenant.ShowNextMaintenanceInPublicPortal = command.ShowNextMaintenanceInPublicPortal;
        tenant.ShowLastMaintenanceInPublicPortal = command.ShowLastMaintenanceInPublicPortal;
        tenant.ShowEquipmentPhotoInPublicPortal = command.ShowEquipmentPhotoInPublicPortal;
        tenant.ShowCompanyInfoInPublicPortal = command.ShowCompanyInfoInPublicPortal;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
