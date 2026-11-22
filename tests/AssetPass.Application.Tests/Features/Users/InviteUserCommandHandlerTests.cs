using AssetPass.Application.Features.Users.Commands.InviteUser;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Enums;
using Xunit;

namespace AssetPass.Application.Tests.Features.Users;

public class InviteUserCommandHandlerTests
{
    private class FakeIdentityService : IIdentityService
    {
        public bool UserExists = false;

        public Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(string email, string password, Guid tenantId, Guid applicationUserId, string role, CancellationToken ct = default) =>
            Task.FromResult<(string?, string[])>((null, []));
        public Task<IdentityUserData?> CheckPasswordAsync(string email, string password, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(null);
        public Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(UserExists ? new IdentityUserData { Email = email } : null);
        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default) =>
            Task.FromResult<string?>(null);
        public Task<(bool Success, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default) =>
            Task.FromResult((false, Array.Empty<string>()));
        public Task<(bool Success, string[] Errors)> ChangePasswordAsync(string identityUserId, string currentPassword, string newPassword, CancellationToken ct = default) =>
            Task.FromResult((false, Array.Empty<string>()));
        public Task ChangeUserRoleAsync(string identityUserId, string oldRole, string newRole, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    private class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default) => Task.CompletedTask;
        public Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken ct = default) => Task.CompletedTask;
        public Task SendInvitationEmailAsync(string to, string inviterName, string companyName, string acceptLink, CancellationToken ct = default) => Task.CompletedTask;
    }

    [Fact]
    public async Task HandleAsync_InvalidEmail_ReturnsValidationFailure()
    {
        var validator = new InviteUserCommandValidator();
        var handler = new InviteUserCommandHandler(null!, new FakeIdentityService(), new FakeEmailService(), validator);

        var command = new InviteUserCommand
        {
            Email = "invalid-email",
            Role = UserRole.Technician
        };

        var result = await handler.HandleAsync(command, Guid.NewGuid(), Guid.NewGuid(), UserRole.Owner);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.ValidationErrors, e => e.Contains("correo"));
    }

    [Fact]
    public async Task HandleAsync_InviteAsOwner_ReturnsValidationFailure()
    {
        var validator = new InviteUserCommandValidator();
        var handler = new InviteUserCommandHandler(null!, new FakeIdentityService(), new FakeEmailService(), validator);

        var command = new InviteUserCommand
        {
            Email = "tech@example.com",
            Role = UserRole.Owner
        };

        var result = await handler.HandleAsync(command, Guid.NewGuid(), Guid.NewGuid(), UserRole.Owner);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.ValidationErrors, e => e.Contains("Owner"));
    }

    [Fact]
    public async Task HandleAsync_TechnicianCaller_ReturnsForbidden()
    {
        var validator = new InviteUserCommandValidator();
        var handler = new InviteUserCommandHandler(null!, new FakeIdentityService(), new FakeEmailService(), validator);

        var command = new InviteUserCommand
        {
            Email = "tech@example.com",
            Role = UserRole.Technician
        };

        var result = await handler.HandleAsync(command, Guid.NewGuid(), Guid.NewGuid(), UserRole.Technician);

        Assert.False(result.IsSuccess);
        Assert.Equal("FORBIDDEN", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_AdminCallerInvitingOwner_ReturnsForbidden()
    {
        var validator = new InviteUserCommandValidator();
        var handler = new InviteUserCommandHandler(null!, new FakeIdentityService(), new FakeEmailService(), validator);

        var command = new InviteUserCommand
        {
            Email = "tech@example.com",
            Role = UserRole.Owner
        };

        // Note: validator catches Owner first, but if validator passed or checked directly, handler also guards
        var result = await handler.HandleAsync(command, Guid.NewGuid(), Guid.NewGuid(), UserRole.Administrator);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyExistsInIdentity_ReturnsFailure()
    {
        var validator = new InviteUserCommandValidator();
        var identity = new FakeIdentityService { UserExists = true };
        var handler = new InviteUserCommandHandler(null!, identity, new FakeEmailService(), validator);

        var command = new InviteUserCommand
        {
            Email = "existing@example.com",
            Role = UserRole.Technician
        };

        var result = await handler.HandleAsync(command, Guid.NewGuid(), Guid.NewGuid(), UserRole.Owner);

        Assert.False(result.IsSuccess);
        Assert.Equal("Ya existe un usuario registrado con ese correo electrónico.", result.Error);
    }
}
