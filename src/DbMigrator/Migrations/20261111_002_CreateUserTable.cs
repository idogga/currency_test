using FluentMigrator;
using TagsAttribute = FluentMigrator.TagsAttribute;

namespace DbMigrator.Migrations;

[Tags(nameof(DatabaseType.Users))]
[Migration(20261111_002)]
public class CreateUserTable : Migration
{
    public override void Up()
    {
        Create.Table("user")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable().Unique()
            .WithColumn("password").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("user");
    }
}
