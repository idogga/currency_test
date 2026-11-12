namespace PersonService.Domain.Interfaces;

/// <summary>
/// Интерфейс для хэширования паролей.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Хэширует пароль.
    /// </summary>
    /// <param name="password">Пароль.</param>
    /// <returns>Хэш пароля.</returns>
    string Hash(string password);

    /// <summary>
    /// Проверяет соответствие пароля и хэша.
    /// </summary>
    /// <param name="password">Пароль.</param>
    /// <param name="hash">Хэш.</param>
    /// <returns>True, если пароль соответствует хэшу, иначе False.</returns>
    bool Verify(string password, string hash);
}