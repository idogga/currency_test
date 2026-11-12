// TestAuthHandler.cs
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonService.Interfaces;

namespace PersonService.Tests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string UserIdHeader = "X-Test-UserId";
    public const string JtiHeader = "X-Test-Jti";

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
            new Claim("exp", expiresAt)   // ← критично: контроллер требует exp
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(principal, SchemeName)));
    }
}