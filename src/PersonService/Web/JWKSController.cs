using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using PersonService.Commands.Login;
using PersonService.Commands.Logout;
using PersonService.Commands.Register;
using PersonService.Commands.ShowAllUsers;
using PersonService.Common.Interfaces;
using PersonService.Dtos;

namespace PersonService.Web;

[ApiController]
[Route(".well-known11111111111111")]
public class JWKSController : ControllerBase
{
    /// <summary>
    /// MediatR для обработки команд.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Поставщик ключей RSA для подписи JWT-токенов.
    /// </summary>
    private readonly IKeyProvider _keyProvider;

    public JWKSController(IMediator mediator, IKeyProvider keyProvider)
    {
        _mediator = mediator;
        _keyProvider = keyProvider;
    }

    /// <summary>
    /// Возвращает JSON Web Key Set (JWKS) для публичного ключа RSA, используемого для подписи JWT-токенов.
    /// </summary>
    /// <returns>JwKS</returns>
    [AllowAnonymous]
    [HttpGet("jwks.json")]
    public async Task<IActionResult> GetJWKS(CancellationToken _)
    {
        var publicParameters = _keyProvider.GetPublicParameters();

        var jwk = new
        {
            kty = "RSA",
            use = "sig",
            alg = "RS256",
            kid = _keyProvider.GetKeyId(),
            n = WebEncoders.Base64UrlEncode(publicParameters.Modulus!),
            e = WebEncoders.Base64UrlEncode(publicParameters.Exponent!)
        };

        return new JsonResult(new { keys = new[] { jwk } });
    }
}