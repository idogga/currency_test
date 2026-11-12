using FinanceService.Infrastructure.Auth;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FinanceService.Application.Mapping;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Domain.Interfaces;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using FinanceService.Application.Queries;
using System.Text.Json;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration missing");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

builder.Services.AddAutoMapper(typeof(CurrencyProfile));
builder.Services.AddSwagger();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();

builder.Services.AddJwtAuthentication(jwtOptions);

builder.Services.AddEndpointsApiExplorer();

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Currency>("Currencies").EntityType.HasKey(c => c.Id);
var edmModel = modelBuilder.GetEdmModel();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100);
        options.AddRouteComponents("odata", edmModel);
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetCurrenciesQuery>());
builder.Services.AddAutoMapper(typeof(CurrencyProfile));

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service v1");
    options.RoutePrefix = "swagger";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }