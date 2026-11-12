using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanceService.Infrastructure.Auth;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FinanceService.Application.Mapping;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Domain.Interfaces;
using Microsoft.OData.ModelBuilder;
using FinanceService.Application.Dtos;
using Microsoft.AspNetCore.OData;
using FinanceService.Application.Queries;
using Microsoft.OpenApi.Models;
using NetDevPack.Security.JwtExtensions;
using System.Text.Json;
using FinanceService.Domain.Entities;

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
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
Console.WriteLine($"JWT Authority: {jwtOptions.Authority}, JWT Audience: {jwtOptions.Audience}, JWT Issuer: {jwtOptions.Issuer}");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtOptions.Authority;
        options.Audience = jwtOptions.Audience;
        // options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        // {
        //     ValidIssuer = jwtOptions.Issuer,
        //     ValidateIssuer = true,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        // };
        options.RequireHttpsMetadata = false;
        // options.TokenValidationParameters.ValidIssuer = "UserService";
        Console.WriteLine($"JWT Authority: {options.Authority}, JWT Audience: {options.Audience}");
        options.SetJwksOptions(new JwkOptions(jwtOptions.Authority + "/jwks"));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finance Service API",
        Version = "v1",
        Description = "Микросервис финансов."
    });

    // JWT Bearer в UI
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Введите JWT без префикса 'Bearer '. Пример: eyJhbGciOi...",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });

    // Подключаем XML-комментарии, если включите их в csproj
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

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