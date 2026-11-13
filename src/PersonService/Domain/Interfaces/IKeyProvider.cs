using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace PersonService.Domain.Interfaces;

/// <summary>
/// Интерфейс для получения публичного ключа RSA, используемого для подписи JWT-токенов.
/// </summary>
public interface IKeyProvider
{
    /// <summary>
    /// Получает публичный ключ RSA для подписи JWT-токенов.
    /// </summary>
    /// <returns>Публичный ключ RSA.</returns>
    RsaSecurityKey GetPublicKey();

    /// <summary>
    /// Получает учетные данные для подписи JWT-токенов.
    /// </summary>
    /// <returns>Учетные данные, содержащие ключ и алгоритм подписи.</returns>
    SigningCredentials GetSigningCredentials();

    /// <summary>
    /// Получает идентификатор публичного ключа.
    /// </summary>
    /// <returns>Идентификатор ключа.</returns>
    string GetKeyId();

    /// <summary>
    /// Получает параметры публичной части RSA-ключа.
    /// </summary>
    /// <returns>Параметры публичного RSA-ключа.</returns>
    RSAParameters GetPublicParameters();
}