using PersonService.Interfaces;

namespace PersonService.Common.Services;

/// <summary>
/// Сервис для хэширования паролей.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc />
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}