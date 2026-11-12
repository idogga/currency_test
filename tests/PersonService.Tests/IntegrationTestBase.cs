// IntegrationTestBase.cs
using System.Net.Http.Json;
using System.Security.Claims;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using PersonService.Dtos;
using PersonService.Infrastructure.Persistence;
using PersonService.Tests.Fixtures;

namespace PersonService.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;
    protected readonly AppDbContext Db;
    protected readonly Faker Faker = new();
    protected readonly IJwtService JwtService;
    private readonly IServiceScope _scope;

    protected IntegrationTestBase(PostgresFixture fixture)
    {
        Factory = new IntegrationTestFactory(fixture.ConnectionString);
        Client = Factory.CreateClient();
        _scope = Factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        JwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    // ---- Хелперы для работы с JWT ----

    /// <summary>Регистрирует нового пользователя и возвращает токен.</summary>
    protected async Task<string> RegisterAndGetTokenAsync(
        string? name = null, string? password = null)
    {
        name ??= Faker.Internet.UserName();
        password ??= "Password123!";

        var response = await Client.PostAsJsonAsync("/api/users",
            new { Name = name, Password = password });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return body!.Token;
    }

    /// <summary>Ставит Authorization: Bearer на клиент.</summary>
    protected void AuthorizeWith(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>Убирает токен с клиента.</summary>
    protected void Unauthorize() => Client.DefaultRequestHeaders.Authorization = null;

    public async Task DisposeAsync()
    {
        try
        {
            await Db.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE \"user\" RESTART IDENTITY CASCADE");
        }
        finally
        {
            Db.Dispose();
            _scope.Dispose();
            Client.Dispose();
            await Factory.DisposeAsync();
        }
    }

    /// <summary>
    /// Генерирует JWT, используя тот же IJwtService, что и само приложение.
    /// Это гарантирует, что подпись будет валидной для текущего экземпляра.
    /// </summary>
    protected string GenerateValidToken(Guid userId, string userName)
    {
        var now = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = IntegrationTestFactory.TestIssuer,
            Audience = IntegrationTestFactory.TestAudience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddHours(1),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            SigningCredentials = JwtService.GetCurrentSigningCredentials().GetAwaiter().GetResult()
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}