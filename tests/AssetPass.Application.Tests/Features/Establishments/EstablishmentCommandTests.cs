using AssetPass.Application.Features.Establishments.Commands.CreateEstablishment;
using AssetPass.Application.Features.Establishments.Commands.UpdateEstablishment;
using Xunit;

namespace AssetPass.Application.Tests.Features.Establishments;

public class EstablishmentCommandTests
{
    [Fact]
    public async Task CreateEstablishmentValidator_EmptyClientId_FailsValidation()
    {
        var validator = new CreateEstablishmentCommandValidator();
        var command = new CreateEstablishmentCommand
        {
            ClientId = Guid.Empty,
            Name = "Sede Principal"
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ClientId");
    }

    [Fact]
    public async Task CreateEstablishmentValidator_EmptyName_FailsValidation()
    {
        var validator = new CreateEstablishmentCommandValidator();
        var command = new CreateEstablishmentCommand
        {
            ClientId = Guid.NewGuid(),
            Name = ""
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateEstablishmentValidator_ValidData_PassesValidation()
    {
        var validator = new CreateEstablishmentCommandValidator();
        var command = new CreateEstablishmentCommand
        {
            ClientId = Guid.NewGuid(),
            Name = "Planta Norte",
            Address = "Av. Circunvalación 450",
            Phone = "809-555-9876",
            ContactPerson = "Ing. Carlos Gómez"
        };

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateEstablishmentValidator_EmptyId_FailsValidation()
    {
        var validator = new UpdateEstablishmentCommandValidator();
        var command = new UpdateEstablishmentCommand
        {
            Id = Guid.Empty,
            ClientId = Guid.NewGuid(),
            Name = "Sede Sur"
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateEstablishmentValidator_EmptyName_FailsValidation()
    {
        var validator = new UpdateEstablishmentCommandValidator();
        var command = new UpdateEstablishmentCommand
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "   "
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateEstablishmentValidator_ValidData_PassesValidation()
    {
        var validator = new UpdateEstablishmentCommandValidator();
        var command = new UpdateEstablishmentCommand
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "Sucursal Central Actualizada",
            Address = "Calle Mayor 123",
            Phone = "809-555-0000",
            IsActive = true
        };

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
