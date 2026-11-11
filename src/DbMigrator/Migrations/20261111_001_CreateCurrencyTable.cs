using FluentMigrator;
using TagsAttribute = FluentMigrator.TagsAttribute;

namespace DbMigrator.Migrations;

[Tags(nameof(DatabaseType.Currency))]
[Migration(20261111_001)]
public class CreateCurrencyTable : Migration
{
    public override void Up()
    {
        Create.Table("currency")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("rate").AsDecimal(28, 12).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("currency");
    }
}
