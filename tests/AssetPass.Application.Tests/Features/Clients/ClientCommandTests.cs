using AssetPass.Application.Features.Clients.Commands.CreateClient;
using AssetPass.Application.Features.Clients.Commands.UpdateClient;
using AssetPass.Domain.Enums;
using Xunit;

namespace AssetPass.Application.Tests.Features.Clients;

public class ClientCommandTests
{
    [Fact]
    public async Task CreateClientValidator_EmptyName_FailsValidation()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand
        {
            Name = "",
            ClientType = ClientType.Business
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateClientValidator_InvalidEmail_FailsValidation()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand
        {
            Name = "Cliente SRL",
            ClientType = ClientType.Business,
            Email = "not-an-email"
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task CreateClientValidator_ValidData_PassesValidation()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand
        {
            Name = "Cliente SRL",
            ClientType = ClientType.Business,
            Email = "contacto@cliente.com",
            Phone = "809-555-1234"
        };

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateClientValidator_EmptyClientId_FailsValidation()
    {
        var validator = new UpdateClientCommandValidator();
        var command = new UpdateClientCommand
        {
            ClientId = Guid.Empty,
            Name = "Cliente Actualizado",
            ClientType = ClientType.Individual
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ClientId");
    }
}
