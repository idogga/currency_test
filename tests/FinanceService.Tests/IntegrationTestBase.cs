using Bogus;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceService.Tests;

/// <summary>
/// Базовый класс для интеграционных тестов.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    /// <summary>
    /// Фабрика для создания тестового приложения.
    /// </summary>
    protected readonly IntegrationTestFactory Factory;

    /// <summary>
    /// HTTP-клиент для отправки запросов к тестовому приложению.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// Контекст базы данных для взаимодействия с тестовой БД.
    /// </summary>
    protected readonly AppDbContext Db;

    /// <summary>
    /// Фейкер для генерации тестовых данных.
    /// </summary>
    protected readonly Faker Faker = new();

    /// <summary>
    /// Сервисный скоуп для управления временем жизни сервисов.
    /// </summary>
    private readonly IServiceScope _scope;

    protected IntegrationTestBase(PostgresFixture fixture)
    {
        Factory = new IntegrationTestFactory(fixture.ConnectionString);
        Client = Factory.CreateClient();
        _scope = Factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Инициализация тестов.
    /// </summary>
    /// <returns></returns>
    public virtual Task InitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Убирает токен с клиента.
    /// </summary>
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

    /// <summary>
    /// Сохраняет переданные валюты в тестовую базу данных.
    /// </summary>
    /// <param name="currencies">Набор валют для сохранения.</param>
    /// <returns></returns>
    protected async Task SeedAsync(params Currency[] currencies)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Currencies.AddRange(currencies);
        await db.SaveChangesAsync();
    }
}
