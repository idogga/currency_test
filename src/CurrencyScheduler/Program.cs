using System.Globalization;
using AutoMapper;
using CurrencyScheduler.Common.Interfaces;
using CurrencyScheduler.Common.Mapping;
using CurrencyScheduler.Common.Services;
using CurrencyScheduler.Infrastructure.Persistence;
using CurrencyScheduler.Jobs;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Polly;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddHttpClient("CbrClient", client =>
{
    client.BaseAddress = new Uri("https://www.cbr.ru/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(
        3,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryCount, context) =>
        {
        }));

builder.Services.AddTransient<ICbrDailyService, CbrXmlDailyService>();
builder.Services.AddTransient<CbrJob>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var host = builder.Build();

var recurringJobManager = host.Services.GetRequiredService<IRecurringJobManager>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

recurringJobManager.AddOrUpdate<CbrJob>(
    "cbr-xml-daily",
    job => job.ExecuteAsync(),
    "0 10 * * *",
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

recurringJobManager.Trigger("cbr-xml-daily");
logger.LogInformation("Задача CBR XML_daily зарегистрирована и запущена немедленно.");

await host.RunAsync();