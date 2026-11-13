using Microsoft.Extensions.Caching.Memory;
using PersonService.Common.Services;

namespace PersonService.Tests.Unit;

/// <summary>
/// Проверяет добавление токенов в черный список и срок их хранения.
/// </summary>
public class TokenBlacklistServiceTests
{
    [Fact]
    public void AddToBlacklist_MarksOnlySpecifiedTokenAsBlacklisted()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new TokenBlacklistService(cache);
        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        service.AddToBlacklist("revoked-token", expiresAt);

        Assert.True(service.IsBlacklisted("revoked-token"));
        Assert.False(service.IsBlacklisted("another-token"));
    }

    [Fact]
    public void AddToBlacklist_DoesNotKeepAlreadyExpiredToken()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new TokenBlacklistService(cache);

        service.AddToBlacklist("expired-token", DateTime.UtcNow.AddSeconds(-1));

        Assert.False(service.IsBlacklisted("expired-token"));
    }
}