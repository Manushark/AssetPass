using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Company.Queries.GetCompanyProfile;

public sealed class GetCompanyProfileQueryHandler
{
    private readonly IApplicationDbContext _db;

    public GetCompanyProfileQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CompanyProfileDto>> HandleAsync(Guid tenantId, CancellationToken ct = default)
    {
        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, ct);

        if (tenant is null)
            return Result<CompanyProfileDto>.NotFound("Empresa no encontrada.");

        var dto = new CompanyProfileDto
        {
            Id = tenant.Id,
            CommercialName = tenant.CommercialName,
            LegalName = tenant.LegalName,
            Rnc = tenant.Rnc,
            Phone = tenant.Phone,
            Email = tenant.Email,
            Address = tenant.Address,
            LogoPath = tenant.LogoPath,
            Currency = tenant.Currency,
            TimeZone = tenant.TimeZone,
            Locale = tenant.Locale,
            SubscriptionStatus = tenant.SubscriptionStatus.ToString(),
            TrialEndDate = tenant.TrialEndDate,
            IsActive = tenant.IsActive,
            ShowWarrantyInPublicPortal = tenant.ShowWarrantyInPublicPortal,
            ShowMaintenanceHistoryInPublicPortal = tenant.ShowMaintenanceHistoryInPublicPortal,
            ShowNextMaintenanceInPublicPortal = tenant.ShowNextMaintenanceInPublicPortal,
            ShowLastMaintenanceInPublicPortal = tenant.ShowLastMaintenanceInPublicPortal,
            ShowEquipmentPhotoInPublicPortal = tenant.ShowEquipmentPhotoInPublicPortal,
            ShowCompanyInfoInPublicPortal = tenant.ShowCompanyInfoInPublicPortal,
        };

        return Result<CompanyProfileDto>.Success(dto);
    }
}
