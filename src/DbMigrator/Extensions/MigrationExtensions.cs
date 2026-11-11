using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace DbMigrator.Extensions;

/// <summary>
/// Расширения для запуска миграций FluentMigrator с использованием тегов.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Запускает миграции FluentMigrator для указанной БД с использованием тегов.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
    /// <param name="connectionString">Строка подключения к БД.</param>
    /// <param name="tag">Тег для фильтрации миграций.</param>
    /// <returns><see cref="IServiceProvider"/></returns>
    public static IServiceProvider RunMigrations(
        this IServiceProvider serviceProvider,
        string connectionString,
        DatabaseType tag)
    {
        var serviceCollection = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .Configure<RunnerOptions>(opt =>
            {
                opt.Tags = [tag.ToString()];
            })
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        var serviceProviderForDb = serviceCollection.BuildServiceProvider();

        using var scope = serviceProviderForDb.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();

        return serviceProvider;
    }
}
