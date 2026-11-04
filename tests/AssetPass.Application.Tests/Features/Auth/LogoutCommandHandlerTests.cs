using AssetPass.Application.Features.Auth.Commands.Logout;
using AssetPass.Application.Interfaces;
using Xunit;

namespace AssetPass.Application.Tests.Features.Auth;

public class LogoutCommandHandlerTests
{
    private class FakeRefreshTokenStore : IRefreshTokenStore
    {
        public readonly List<string> RevokedTokens = new();
        public readonly List<string> RevokedUsers = new();
        public readonly Dictionary<string, string> StoredTokens = new(); // token -> userId

        public Task StoreAsync(string identityUserId, string refreshToken, DateTime expiresAt, CancellationToken ct = default)
        {
            StoredTokens[refreshToken] = identityUserId;
            return Task.CompletedTask;
        }

        public Task<string?> ValidateAndGetUserIdAsync(string refreshToken, CancellationToken ct = default)
        {
            return Task.FromResult(StoredTokens.TryGetValue(refreshToken, out var uid) ? uid : null);
        }

        public Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            RevokedTokens.Add(refreshToken);
            StoredTokens.Remove(refreshToken);
            return Task.CompletedTask;
        }

        public Task RevokeAllAsync(string identityUserId, CancellationToken ct = default)
        {
            RevokedUsers.Add(identityUserId);
            var toRemove = StoredTokens.Where(kvp => kvp.Value == identityUserId).Select(kvp => kvp.Key).ToList();
            foreach (var k in toRemove)
                StoredTokens.Remove(k);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task HandleAsync_WithRefreshToken_RevokesSingleToken()
    {
        // Arrange
        var store = new FakeRefreshTokenStore();
        await store.StoreAsync("user-123", "token-abc", DateTime.UtcNow.AddDays(7));
        var handler = new LogoutCommandHandler(store);
        var command = new LogoutCommand { RefreshToken = "token-abc", RevokeAllSessions = false };

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("token-abc", store.RevokedTokens);
        Assert.Empty(store.RevokedUsers);
    }

    [Fact]
    public async Task HandleAsync_WithRevokeAllSessions_RevokesAllUserTokens()
    {
        // Arrange
        var store = new FakeRefreshTokenStore();
        await store.StoreAsync("user-123", "token-abc", DateTime.UtcNow.AddDays(7));
        var handler = new LogoutCommandHandler(store);
        var command = new LogoutCommand { RefreshToken = "token-abc", RevokeAllSessions = true };

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("user-123", store.RevokedUsers);
    }

    [Fact]
    public async Task HandleAsync_WithAuthenticatedUser_RevokesAllSessionsWhenRequested()
    {
        // Arrange
        var store = new FakeRefreshTokenStore();
        var handler = new LogoutCommandHandler(store);
        var command = new LogoutCommand { RevokeAllSessions = true };

        // Act
        var result = await handler.HandleAsync(command, authenticatedIdentityUserId: "user-456");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("user-456", store.RevokedUsers);
    }

    [Fact]
    public async Task HandleAsync_WithoutContextOrToken_ReturnsFailure()
    {
        // Arrange
        var store = new FakeRefreshTokenStore();
        var handler = new LogoutCommandHandler(store);
        var command = new LogoutCommand();

        // Act
        var result = await handler.HandleAsync(command, authenticatedIdentityUserId: null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
}
