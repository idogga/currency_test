namespace FinanceService.Tests.Fixtures;

[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres-collection";
}