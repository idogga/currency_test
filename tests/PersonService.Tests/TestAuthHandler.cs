using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonService.Domain.Interfaces;

namespace PersonService.Tests;

/// <summary>
/// Хэндлер для тестовой авторизации.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Название схемы.
    /// </summary>
    public const string SchemeName = "Test";

    /// <summary>
    /// Название хэдера для User-Id.
    /// </summary>
    public const string UserIdHeader = "X-Test-UserId";

    /// <summary>
    /// Название хэдера для Оеш.
    /// </summary>
    public const string JtiHeader = "X-Test-Jti";

    /// <summary>
    /// Сервис для работы с заблокированными токенами.
    /// </summary>
    private readonly ITokenBlacklistService _blacklist;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITokenBlacklistService blacklist) : base(options, logger, encoder)
    {
        _blacklist = blacklist;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(UserIdHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        var jti = Request.Headers[JtiHeader].FirstOrDefault()
                  ?? Guid.NewGuid().ToString();

        if (_blacklist.IsBlacklisted(jti))
            return Task.FromResult(AuthenticateResult.Fail("Token is blacklisted."));

        var expiresAt = DateTimeOffset.UtcNow
            .AddHours(1)
            .ToUnixTimeSeconds()
            .ToString();

        var claims = new[]
        {
            new Claim("sub", Request.Headers[UserIdHeader]!),
            new Claim("jti", jti),
            new Claim("exp", expiresAt)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(principal, SchemeName)));
    }
}