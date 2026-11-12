using Testcontainers.PostgreSql;
using Xunit;

namespace PersonService.Tests.Fixtures;

/// <summary>
/// Сервис по управлению Postgres.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;

    /// <summary>
    /// Строка подключения к БД.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    /// Запуск.
    /// </summary>
    /// <returns>Задача.</returns>
    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("users_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}