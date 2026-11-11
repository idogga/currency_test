using DbMigrator;
using DbMigrator.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var usersConnectionString = builder.Configuration.GetConnectionString("UsersDb");
var financyConnectionString = builder.Configuration.GetConnectionString("FinancyDb");

if (string.IsNullOrEmpty(usersConnectionString) || string.IsNullOrEmpty(financyConnectionString))
{
    throw new InvalidOperationException("Connection strings are not configured.");
}

app.Services.RunMigrations(usersConnectionString, DatabaseType.Users);
app.Services.RunMigrations(financyConnectionString, DatabaseType.Currency);
