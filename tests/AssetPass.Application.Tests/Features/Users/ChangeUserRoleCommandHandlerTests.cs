using AssetPass.Application.Features.Users.Commands.ChangeUserRole;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Enums;
using Xunit;

namespace AssetPass.Application.Tests.Features.Users;

public class ChangeUserRoleCommandHandlerTests
{
    private class FakeIdentityService : IIdentityService
    {
        public Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(string email, string password, Guid tenantId, Guid applicationUserId, string role, CancellationToken ct = default) =>
            Task.FromResult<(string?, string[])>((null, []));
        public Task<IdentityUserData?> CheckPasswordAsync(string email, string password, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(null);
        public Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(null);
        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default) =>
            Task.FromResult<string?>(null);
        public Task<(bool Success, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default) =>
            Task.FromResult((false, Array.Empty<string>()));
        public Task<(bool Success, string[] Errors)> ChangePasswordAsync(string identityUserId, string currentPassword, string newPassword, CancellationToken ct = default) =>
            Task.FromResult((false, Array.Empty<string>()));
        public Task ChangeUserRoleAsync(string identityUserId, string oldRole, string newRole, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    [Fact]
    public async Task HandleAsync_SelfRoleChange_ReturnsFailure()
    {
        var identity = new FakeIdentityService();
        var handler = new ChangeUserRoleCommandHandler(null!, identity);
        var targetUserId = Guid.NewGuid();

        var result = await handler.HandleAsync(
            new ChangeUserRoleCommand { TargetUserId = targetUserId, NewRole = UserRole.Technician },
            callerTenantId: Guid.NewGuid(),
            callerApplicationUserId: targetUserId, // Same as target
            callerRole: UserRole.Owner);

        Assert.False(result.IsSuccess);
        Assert.Equal("No puedes cambiar tu propio rol.", result.Error);
    }

    [Fact]
    public async Task HandleAsync_TechnicianCaller_ReturnsForbidden()
    {
        var identity = new FakeIdentityService();
        var handler = new ChangeUserRoleCommandHandler(null!, identity);

        var result = await handler.HandleAsync(
            new ChangeUserRoleCommand { TargetUserId = Guid.NewGuid(), NewRole = UserRole.Administrator },
            callerTenantId: Guid.NewGuid(),
            callerApplicationUserId: Guid.NewGuid(),
            callerRole: UserRole.Technician); // Technician cannot change roles

        Assert.False(result.IsSuccess);
        Assert.Equal("FORBIDDEN", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_AdminAssigningOwner_ReturnsForbidden()
    {
        var identity = new FakeIdentityService();
        var handler = new ChangeUserRoleCommandHandler(null!, identity);

        var result = await handler.HandleAsync(
            new ChangeUserRoleCommand { TargetUserId = Guid.NewGuid(), NewRole = UserRole.Owner }, // Admin trying to assign Owner
            callerTenantId: Guid.NewGuid(),
            callerApplicationUserId: Guid.NewGuid(),
            callerRole: UserRole.Administrator);

        Assert.False(result.IsSuccess);
        Assert.Equal("FORBIDDEN", result.ErrorCode);
    }
}

