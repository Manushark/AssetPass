using AssetPass.Infrastructure.Services;
using Xunit;

namespace AssetPass.Infrastructure.Tests.Services;

public class InMemoryRefreshTokenStoreTests
{
    [Fact]
    public async Task StoreAsync_And_ValidateAndGetUserIdAsync_ValidToken_ReturnsUserId()
    {
        var store = new InMemoryRefreshTokenStore();
        var userId = "user-123";
        var token = "refresh-token-abc";
        var expiresAt = DateTime.UtcNow.AddMinutes(30);

        await store.StoreAsync(userId, token, expiresAt);
        var retrievedUserId = await store.ValidateAndGetUserIdAsync(token);

        Assert.Equal(userId, retrievedUserId);
    }

    [Fact]
    public async Task ValidateAndGetUserIdAsync_ExpiredToken_ReturnsNullAndCleansUp()
    {
        var store = new InMemoryRefreshTokenStore();
        var token = "expired-token";
        var expiresAt = DateTime.UtcNow.AddMinutes(-5);

        await store.StoreAsync("user-456", token, expiresAt);
        var result = await store.ValidateAndGetUserIdAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task RevokeTokenAsync_RemovesSpecificToken()
    {
        var store = new InMemoryRefreshTokenStore();
        var token = "token-to-revoke";

        await store.StoreAsync("user-789", token, DateTime.UtcNow.AddHours(1));
        await store.RevokeTokenAsync(token);
        var result = await store.ValidateAndGetUserIdAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task RevokeAllAsync_RemovesAllTokensForGivenUser()
    {
        var store = new InMemoryRefreshTokenStore();
        var user1 = "user-multi-session";
        var user2 = "user-other";

        await store.StoreAsync(user1, "tok-1", DateTime.UtcNow.AddDays(1));
        await store.StoreAsync(user1, "tok-2", DateTime.UtcNow.AddDays(1));
        await store.StoreAsync(user2, "tok-3", DateTime.UtcNow.AddDays(1));

        await store.RevokeAllAsync(user1);

        Assert.Null(await store.ValidateAndGetUserIdAsync("tok-1"));
        Assert.Null(await store.ValidateAndGetUserIdAsync("tok-2"));
        Assert.NotNull(await store.ValidateAndGetUserIdAsync("tok-3"));
    }
}

