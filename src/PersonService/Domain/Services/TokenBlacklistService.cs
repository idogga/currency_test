using Microsoft.Extensions.Caching.Memory;
using PersonService.Domain.Interfaces;

namespace PersonService.Common.Services;

/// <summary>
/// Хранит идентификаторы отозванных токенов в кэше до истечения их срока действия.
/// </summary>
public class TokenBlacklistService : ITokenBlacklistService
{
    /// <summary>
    /// Кэш для хранения заблокированных токенов.
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Создает сервис черного списка токенов.
    /// </summary>
    /// <param name="cache">Кэш для хранения идентификаторов отозванных токенов.</param>
    public TokenBlacklistService(IMemoryCache cache) => _cache = cache;

    /// <summary>
    /// Добавляет идентификатор токена в черный список до указанного срока истечения.
    /// </summary>
    /// <param name="jti">Уникальный идентификатор JWT.</param>
    /// <param name="expiresAt">Дата и время истечения токена.</param>
    public void AddToBlacklist(string jti, DateTime expiresAt)
    {
        _cache.Set($"blacklist:{jti}", true, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiresAt
        });
    }

    /// <summary>
    /// Проверяет, находится ли идентификатор токена в черном списке.
    /// </summary>
    /// <param name="jti">Уникальный идентификатор JWT.</param>
    /// <returns><see langword="true"/>, если токен заблокирован; иначе <see langword="false"/>.</returns>
    public bool IsBlacklisted(string jti)
    {
        return _cache.TryGetValue($"blacklist:{jti}", out _);
    }
}