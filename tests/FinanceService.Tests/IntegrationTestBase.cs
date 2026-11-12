// IntegrationTestBase.cs
using System.Net.Http.Json;
using System.Security.Claims;
using Bogus;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FinanceService.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;
    protected readonly AppDbContext Db;
    protected readonly Faker Faker = new();
    private readonly IServiceScope _scope;

    protected IntegrationTestBase(PostgresFixture fixture)
    {
        Factory = new IntegrationTestFactory(fixture.ConnectionString);
        Client = Factory.CreateClient();
        _scope = Factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    /// <summary>Убирает токен с клиента.</summary>
    protected void Unauthorize() => Client.DefaultRequestHeaders.Authorization = null;

    public async Task DisposeAsync()
    {
        try
        {
            await Db.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE \"Currencies\" RESTART IDENTITY CASCADE");
        }
        finally
        {
            Db.Dispose();
            _scope.Dispose();
            Client.Dispose();
            await Factory.DisposeAsync();
        }
    }

    protected async Task SeedAsync(params Currency[] currencies)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Currencies.AddRange(currencies);
        await db.SaveChangesAsync();
    }
}
