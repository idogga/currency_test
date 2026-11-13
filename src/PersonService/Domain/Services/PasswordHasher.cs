using PersonService.Domain.Interfaces;

namespace PersonService.Common.Services;

/// <summary>
/// Сервис для хэширования паролей.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Создает криптографический хэш пароля.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Хэш пароля с солью.</returns>
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Проверяет пароль на соответствие сохраненному хэшу.
    /// </summary>
    /// <param name="password">Проверяемый пароль.</param>
    /// <param name="hash">Сохраненный хэш пароля.</param>
    /// <returns><see langword="true"/>, если пароль соответствует хэшу; иначе <see langword="false"/>.</returns>
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}