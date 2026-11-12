namespace FinanceService.Tests.Fixtures;

/// <summary>
/// Коллекция для тестов, использующих PostgreSQL.
/// </summary>
[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    /// <summary>
    /// Имя коллекции для тестов, использующих PostgreSQL.
    /// </summary>
    public const string Name = "postgres-collection";
}