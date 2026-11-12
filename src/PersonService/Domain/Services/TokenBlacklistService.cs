using Microsoft.Extensions.Caching.Memory;
using PersonService.Domain.Interfaces;

namespace PersonService.Common.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    /// <summary>
    /// Кэш для хранения заблокированных токенов.
    /// </summary>
    private readonly IMemoryCache _cache;

    public TokenBlacklistService(IMemoryCache cache) => _cache = cache;

    /// <inheritdoc />
    public void AddToBlacklist(string jti, DateTime expiresAt)
    {
        _cache.Set($"blacklist:{jti}", true, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiresAt
        });
    }

    /// <inheritdoc />
    public bool IsBlacklisted(string jti)
    {
        return _cache.TryGetValue($"blacklist:{jti}", out _);
    }
}