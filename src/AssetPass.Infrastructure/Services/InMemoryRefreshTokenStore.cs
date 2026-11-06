using System.Collections.Concurrent;
using AssetPass.Application.Interfaces;

namespace AssetPass.Infrastructure.Services;

/// <summary>
/// In-memory refresh token store for development and testing.
///
/// IMPORTANT: This is NOT production-ready. It does not survive restarts and
/// does not scale across multiple API instances. Replace with a database-backed
/// implementation (e.g., Redis or a RefreshTokens table) before going to production.
///
/// Thread-safe via ConcurrentDictionary.
/// </summary>
public sealed class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private sealed record TokenEntry(string IdentityUserId, DateTime ExpiresAt);

    // token string → entry
    private readonly ConcurrentDictionary<string, TokenEntry> _tokens = new();

    /// <inheritdoc />
    public Task StoreAsync(string identityUserId, string refreshToken, DateTime expiresAt, CancellationToken ct = default)
    {
        _tokens[refreshToken] = new TokenEntry(identityUserId, expiresAt);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> ValidateAndGetUserIdAsync(string refreshToken, CancellationToken ct = default)
    {
        if (_tokens.TryGetValue(refreshToken, out var entry))
        {
            if (entry.ExpiresAt > DateTime.UtcNow)
                return Task.FromResult<string?>(entry.IdentityUserId);

            // Expired — clean up
            _tokens.TryRemove(refreshToken, out _);
        }

        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc />
    public Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        _tokens.TryRemove(refreshToken, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RevokeAllAsync(string identityUserId, CancellationToken ct = default)
    {
        var toRemove = _tokens
            .Where(kvp => kvp.Value.IdentityUserId == identityUserId)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in toRemove)
            _tokens.TryRemove(key, out _);

        return Task.CompletedTask;
    }
}

