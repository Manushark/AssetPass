using AssetPass.Application.Features.Company.Commands.UpdateCompanyProfile;
using Xunit;

namespace AssetPass.Application.Tests.Features.Company;

public class CompanyCommandTests
{
    [Fact]
    public async Task UpdateCompanyProfileValidator_EmptyName_ReturnsValidationError()
    {
        var validator = new UpdateCompanyProfileCommandValidator();
        var command = new UpdateCompanyProfileCommand
        {
            CommercialName = "",
            Phone = "809-555-0100",
            Email = "company@assetpass.com",
            Currency = "DOP",
            TimeZone = "America/Santo_Domingo"
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CommercialName");
    }

    [Fact]
    public async Task UpdateCompanyProfileValidator_InvalidCurrency_ReturnsValidationError()
    {
        var validator = new UpdateCompanyProfileCommandValidator();
        var command = new UpdateCompanyProfileCommand
        {
            CommercialName = "Empresa Valida",
            Phone = "809-555-0100",
            Email = "company@assetpass.com",
            Currency = "DO", // Invalid (needs 3 characters)
            TimeZone = "America/Santo_Domingo"
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Currency");
    }

    [Fact]
    public async Task UpdateCompanyProfileValidator_ValidCommand_PassesValidation()
    {
        var validator = new UpdateCompanyProfileCommandValidator();
        var command = new UpdateCompanyProfileCommand
        {
            CommercialName = "Empresa Valida",
            Phone = "809-555-0100",
            Email = "company@assetpass.com",
            Currency = "DOP",
            TimeZone = "America/Santo_Domingo"
        };

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
