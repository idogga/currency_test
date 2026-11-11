namespace PersonService.Interfaces;

/// <summary>
/// Сервис для генерации JWT-токенов.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Генерирует JWT-токен для пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="jti">JWT ID.</param>
    /// <param name="expiresAt">Дата истечения токена.</param>
    /// <returns>JWT-токен.</returns>
    string GenerateToken(Guid userId, string userName, out string jti, out DateTime expiresAt);
}