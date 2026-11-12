using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using FinanceService.Infrastructure.Persistence;

namespace FinanceService.Tests;

/// <summary>
/// Фабрика для интеграционных тестов.
/// </summary>
public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Имя тестового издателя JWT.
    /// </summary>
    public const string TestIssuer = "UserService";

    /// <summary>
    /// Имя тестовой аудитории JWT.
    /// </summary>
    public const string TestAudience = "UserServiceClients";

    /// <summary>
    /// Фабрика для интеграционных тестов.
    /// </summary>
    private readonly string _connectionString;

    public IntegrationTestFactory(string connectionString) => _connectionString = connectionString;

    /// <summary>
    /// Конфигурирует веб-хост для интеграционных тестов.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Jwt:Issuer"] = TestIssuer,
                ["Jwt:Audience"] = TestAudience
            });
        });

        builder.ConfigureServices(services =>
        {
            // Заменяем DbContext на тестовую БД
            var toRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                         || d.ServiceType == typeof(AppDbContext))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseNpgsql(_connectionString));

            // Создаём схему
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        });

        builder.ConfigureTestServices(services =>
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TestIssuer,
                        ValidateAudience = true,
                        ValidAudience = TestAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = false,
                        SignatureValidator = (token, _) =>
                            new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token)
                    };

                    // Не перезаписываем наши TVP настройками NetDevPack
                    options.ConfigurationManager = null;
                });
        });
    }
}