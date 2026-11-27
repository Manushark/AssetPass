using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;

public sealed class UpdateCompanyProfileCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<UpdateCompanyProfileCommand> _validator;

    public UpdateCompanyProfileCommandHandler(
        IApplicationDbContext db,
        IValidator<UpdateCompanyProfileCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(
        UpdateCompanyProfileCommand command,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null)
            return Result.NotFound("Empresa no encontrada.");

        tenant.CommercialName = command.CommercialName.Trim();
        tenant.LegalName = command.LegalName?.Trim();
        tenant.Rnc = command.Rnc?.Trim();
        tenant.Phone = command.Phone.Trim();
        tenant.Email = command.Email.Trim().ToLowerInvariant();
        tenant.Address = command.Address?.Trim();
        tenant.Currency = command.Currency.Trim().ToUpperInvariant();
        tenant.TimeZone = command.TimeZone.Trim();
        tenant.Locale = command.Locale.Trim();

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
