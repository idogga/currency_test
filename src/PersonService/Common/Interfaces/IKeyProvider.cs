using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace PersonService.Common.Interfaces;

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

    SigningCredentials GetSigningCredentials();

    string GetKeyId();

    RSAParameters GetPublicParameters();
}