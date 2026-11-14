using AssetPass.Application.Features.Auth.Commands.ChangePassword;
using AssetPass.Application.Features.Auth.Commands.ForgotPassword;
using AssetPass.Application.Features.Auth.Commands.ResetPassword;
using AssetPass.Application.Interfaces;
using Xunit;

namespace AssetPass.Application.Tests.Features.Auth;

public class PasswordFlowTests
{
    private class FakeEmailService : IEmailService
    {
        public readonly List<(string To, string ResetLink)> SentResets = new();

        public Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default) =>
            Task.CompletedTask;

        public Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken ct = default)
        {
            SentResets.Add((to, resetLink));
            return Task.CompletedTask;
        }

        public Task SendInvitationEmailAsync(string to, string inviterName, string companyName, string acceptLink, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    private class FakeIdentityService : IIdentityService
    {
        public string? TokenToGenerate = "reset-token-123";
        public bool ResetSucceeds = true;
        public bool ChangeSucceeds = true;
        public string? LastResetTokenUsed;
        public string? LastNewPasswordUsed;

        public Task<(string? IdentityUserId, string[] Errors)> CreateUserAsync(string email, string password, Guid tenantId, Guid applicationUserId, string role, CancellationToken ct = default) =>
            Task.FromResult<(string?, string[])>(("user-id", []));

        public Task<IdentityUserData?> CheckPasswordAsync(string email, string password, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(null);

        public Task<IdentityUserData?> FindByEmailAsync(string email, CancellationToken ct = default) =>
            Task.FromResult<IdentityUserData?>(new IdentityUserData { IdentityUserId = "id-1", Email = email });

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default) =>
            Task.FromResult(TokenToGenerate);

        public Task<(bool Success, string[] Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
        {
            LastResetTokenUsed = token;
            LastNewPasswordUsed = newPassword;
            return Task.FromResult(ResetSucceeds ? (true, Array.Empty<string>()) : (false, new[] { "Invalid token" }));
        }

        public Task<(bool Success, string[] Errors)> ChangePasswordAsync(string identityUserId, string currentPassword, string newPassword, CancellationToken ct = default) =>
            Task.FromResult(ChangeSucceeds ? (true, Array.Empty<string>()) : (false, new[] { "Incorrect current password" }));

        public Task ChangeUserRoleAsync(string identityUserId, string oldRole, string newRole, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    private class FakeRefreshTokenStore : IRefreshTokenStore
    {
        public readonly List<string> RevokedUsers = new();

        public Task StoreAsync(string identityUserId, string refreshToken, DateTime expiresAt, CancellationToken ct = default) => Task.CompletedTask;
        public Task<string?> ValidateAndGetUserIdAsync(string refreshToken, CancellationToken ct = default) => Task.FromResult<string?>(null);
        public Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default) => Task.CompletedTask;
        public Task RevokeAllAsync(string identityUserId, CancellationToken ct = default)
        {
            RevokedUsers.Add(identityUserId);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ForgotPassword_ValidEmail_SendsResetEmail()
    {
        var identity = new FakeIdentityService();
        var emailSvc = new FakeEmailService();
        var validator = new ForgotPasswordCommandValidator();
        var handler = new ForgotPasswordCommandHandler(identity, emailSvc, validator);

        var result = await handler.HandleAsync(new ForgotPasswordCommand { Email = "user@test.com" });

        Assert.True(result.IsSuccess);
        Assert.Single(emailSvc.SentResets);
        Assert.Equal("user@test.com", emailSvc.SentResets[0].To);
    }

    [Fact]
    public async Task ForgotPassword_InvalidEmail_FailsValidation()
    {
        var identity = new FakeIdentityService();
        var emailSvc = new FakeEmailService();
        var validator = new ForgotPasswordCommandValidator();
        var handler = new ForgotPasswordCommandHandler(identity, emailSvc, validator);

        var result = await handler.HandleAsync(new ForgotPasswordCommand { Email = "not-an-email" });

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.ValidationErrors);
    }

    [Fact]
    public async Task ResetPassword_ValidToken_ResetsAndRevokesTokens()
    {
        var identity = new FakeIdentityService();
        var store = new FakeRefreshTokenStore();
        var validator = new ResetPasswordCommandValidator();
        var handler = new ResetPasswordCommandHandler(identity, store, validator);

        var command = new ResetPasswordCommand
        {
            Email = "user@test.com",
            Token = "reset-token-123",
            NewPassword = "NewPassword123!",
            NewPasswordConfirm = "NewPassword123!"
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal("reset-token-123", identity.LastResetTokenUsed);
        Assert.Contains("id-1", store.RevokedUsers);
    }

    [Fact]
    public async Task ChangePassword_ValidInputs_ChangesAndRevokesTokens()
    {
        var identity = new FakeIdentityService();
        var store = new FakeRefreshTokenStore();
        var validator = new ChangePasswordCommandValidator();
        var handler = new ChangePasswordCommandHandler(identity, store, validator);

        var command = new ChangePasswordCommand
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            NewPasswordConfirm = "NewPassword123!"
        };

        var result = await handler.HandleAsync(command, "user-id-99");

        Assert.True(result.IsSuccess);
        Assert.Contains("user-id-99", store.RevokedUsers);
    }

    [Fact]
    public async Task ChangePassword_Unauthenticated_ReturnsUnauthorized()
    {
        var identity = new FakeIdentityService();
        var store = new FakeRefreshTokenStore();
        var validator = new ChangePasswordCommandValidator();
        var handler = new ChangePasswordCommandHandler(identity, store, validator);

        var command = new ChangePasswordCommand
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            NewPasswordConfirm = "NewPassword123!"
        };

        var result = await handler.HandleAsync(command, string.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal("UNAUTHORIZED", result.ErrorCode);
    }
}

