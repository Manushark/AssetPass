using AssetPass.Application.Features.Auth.Commands.AcceptInvitation;
using AssetPass.Application.Interfaces;
using Xunit;

namespace AssetPass.Application.Tests.Features.Auth;

public class AcceptInvitationCommandHandlerTests
{
    private class FakeIdentityService : IIdentityService
    {
        public Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(string email, string password, Guid tenantId, Guid applicationUserId, string role, CancellationToken ct = default) =>
            Task.FromResult<(string?, string[])>(("user-id", []));
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
    public async Task HandleAsync_EmptyToken_ReturnsValidationFailure()
    {
        var validator = new AcceptInvitationCommandValidator();
        var handler = new AcceptInvitationCommandHandler(null!, new FakeIdentityService(), validator);

        var command = new AcceptInvitationCommand
        {
            Token = "",
            Password = "ValidPassword123!",
            ConfirmPassword = "ValidPassword123!"
        };

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.ValidationErrors, e => e.Contains("token"));
    }

    [Fact]
    public async Task HandleAsync_PasswordMismatch_ReturnsValidationFailure()
    {
        var validator = new AcceptInvitationCommandValidator();
        var handler = new AcceptInvitationCommandHandler(null!, new FakeIdentityService(), validator);

        var command = new AcceptInvitationCommand
        {
            Token = "valid-token",
            Password = "ValidPassword123!",
            ConfirmPassword = "DifferentPassword456!"
        };

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.ValidationErrors, e => e.Contains("coinciden"));
    }

    [Fact]
    public async Task HandleAsync_WeakPassword_ReturnsValidationFailure()
    {
        var validator = new AcceptInvitationCommandValidator();
        var handler = new AcceptInvitationCommandHandler(null!, new FakeIdentityService(), validator);

        var command = new AcceptInvitationCommand
        {
            Token = "valid-token",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.True(result.ValidationErrors.Count > 0);
    }
}
