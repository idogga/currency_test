using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AppDbContext = FinanceService.Infrastructure.Persistence.AppDbContext;
using Bogus;
using FinanceService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using FinanceService.Tests.Fixtures;
using FinanceService.Application.Dtos;

namespace FinanceService.Tests.Endpoints;

[Collection(PostgresCollection.Name)]
public class CurrenciesEndpointTests : IntegrationTestBase
{
    public CurrenciesEndpointTests(PostgresFixture fixture) : base(fixture)
    {

    }

    private readonly Faker<Currency> _currencyFaker = new Faker<Currency>()
        .RuleFor(c => c.Id, f => Guid.NewGuid())
        .RuleFor(c => c.Name, f => f.Finance.Currency().Code)
        .RuleFor(c => c.Rate, f => f.Finance.Amount(1, 200));

    private void Authenticate() =>
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

    // ---------- 1. Без токена → 401 ----------
    [Fact]
    public async Task GetCurrencies_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/odata/Currencies");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---------- 2. С токеном, пустая БД → 200, пустой массив ----------
    [Fact]
    public async Task GetCurrencies_WithAuth_EmptyDb_ReturnsEmpty()
    {
        Authenticate();

        var response = await Client.GetAsync("/api/currencies");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        Assert.NotNull(payload);
        Assert.Empty(payload!);
    }

    // ---------- 3. С токеном и данными → 200, все валюты ----------
    [Fact]
    public async Task GetCurrencies_WithAuthAndData_ReturnsAll()
    {
        var seeded = _currencyFaker.Generate(3);
        await SeedAsync(seeded.ToArray());
        Authenticate();

        var response = await Client.GetAsync("/odata/Currencies");
        var a = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        Assert.NotNull(payload);
        Assert.Equal(seeded.Count, payload.Count);
        Assert.Equal(
            seeded.Select(c => c.Name).OrderBy(n => n),
            payload.Select(c => c.Name).OrderBy(n => n));
    }

    // ---------- 4. OData $filter → только подходящие записи ----------
    [Fact]
    public async Task GetCurrencies_WithFilter_ReturnsFiltered()
    {
        var cheap = new Currency { Id = Guid.NewGuid(), Name = "CHEAP", Rate = 10m };
        var expensive = new Currency { Id = Guid.NewGuid(), Name = "EXPENSIVE", Rate = 150m };
        await SeedAsync(cheap, expensive);
        Authenticate();

        var response = await Client.GetAsync("/odata/Currencies?$filter=Rate gt 50");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        Assert.NotNull(payload);
        var item = Assert.Single(payload);
        Assert.Equal("EXPENSIVE", item.Name);
    }

    // ---------- 5. OData $orderby + $top → сортировка и ограничение ----------
    [Fact]
    public async Task GetCurrencies_WithOrderByAndTop_ReturnsOrderedAndLimited()
    {
        var currencies = new[]
        {
            new Currency { Id = Guid.NewGuid(), Name = "C", Rate = 30m },
            new Currency { Id = Guid.NewGuid(), Name = "A", Rate = 10m },
            new Currency { Id = Guid.NewGuid(), Name = "B", Rate = 20m }
        };
        await SeedAsync(currencies);
        Authenticate();

        var response = await Client.GetAsync("/odata/Currencies?$orderby=Rate asc&$top=2");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload.Count);
        Assert.Equal(new[] { "A", "B" }, payload.Select(c => c.Name));
    }
}