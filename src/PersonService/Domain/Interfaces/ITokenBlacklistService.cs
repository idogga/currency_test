namespace PersonService.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с черным списком токенов.
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// Добавляет токен в черный список.
    /// </summary>
    /// <param name="jti">JWT ID.</param>
    /// <param name="expiresAt">Дата истечения токена.</param>
    void AddToBlacklist(string jti, DateTime expiresAt);

    /// <summary>
    /// Проверяет, находится ли токен в черном списке.
    /// </summary>
    /// <param name="jti">JWT ID.</param>
    /// <returns>True, если токен находится в черном списке, иначе False.</returns>
    bool IsBlacklisted(string jti);
}