using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using PersonService.Domain.Interfaces;

namespace PersonService.Common.Services;

/// <summary>
/// Создает и предоставляет RSA-ключи для подписи и проверки JWT-токенов.
/// </summary>
public sealed class RsaKeyProvider : IKeyProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly string _keyId;

    /// <summary>
    /// Создает новую пару RSA-ключей и вычисляет идентификатор публичного ключа.
    /// </summary>
    public RsaKeyProvider()
    {
        _rsa = RSA.Create(2048);
        _keyId = Base64UrlEncoder.Encode(SHA256.HashData(_rsa.ExportRSAPublicKey()));
    }

    /// <summary>
    /// Возвращает параметры публичной части RSA-ключа.
    /// </summary>
    /// <returns>Параметры публичного RSA-ключа.</returns>
    public RSAParameters GetPublicParameters()
    {
        return _rsa.ExportParameters(includePrivateParameters: false);
    }

    /// <summary>
    /// Возвращает публичный RSA-ключ с его идентификатором.
    /// </summary>
    /// <returns>Публичный ключ для проверки подписи JWT.</returns>
    public RsaSecurityKey GetPublicKey()
    {
        return new(_rsa) { KeyId = _keyId };
    }

    /// <summary>
    /// Возвращает идентификатор публичного ключа.
    /// </summary>
    /// <returns>Идентификатор ключа в формате Base64Url.</returns>
    public string GetKeyId()
    {
        return _keyId;
    }

    /// <summary>
    /// Создает учетные данные подписи на основе RSA-ключа.
    /// </summary>
    /// <returns>Учетные данные для подписи JWT алгоритмом RSA SHA-256.</returns>
    public SigningCredentials GetSigningCredentials()
    {
        var publicKey = GetPublicKey();
        return new SigningCredentials(publicKey, SecurityAlgorithms.RsaSha256);
    }

    /// <summary>
    /// Освобождает ресурсы RSA-ключа.
    /// </summary>
    public void Dispose() => _rsa.Dispose();
}