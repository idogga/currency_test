using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using PersonService.Common.Interfaces;

namespace PersonService.Common.Services;

/// <inheritdoc />
public sealed class RsaKeyProvider : IKeyProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly string _keyId;

    public RsaKeyProvider()
    {
        _rsa = RSA.Create(2048);
        _keyId = Base64UrlEncoder.Encode(SHA256.HashData(_rsa.ExportRSAPublicKey()));
    }

    public RSAParameters GetPublicParameters()
    {
        return _rsa.ExportParameters(includePrivateParameters: false);
    }

    public RsaSecurityKey GetPublicKey()
    {
        return new(_rsa) { KeyId = _keyId };
    }

    public string GetKeyId()
    {
        return _keyId;
    }

    public SigningCredentials GetSigningCredentials()
    {
        var publicKey = GetPublicKey();
        return new SigningCredentials(publicKey, SecurityAlgorithms.RsaSha256);
    }

    public void Dispose() => _rsa.Dispose();
}