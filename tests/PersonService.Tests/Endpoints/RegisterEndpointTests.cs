using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using PersonService.Application.Dtos;
using PersonService.Tests.Fixtures;

namespace PersonService.Tests.Endpoints;

/// <summary>
/// Тесты для эндпоинта регистрации.
/// </summary>
[Collection(PostgresCollection.Name)]
public class RegisterEndpointTests : IntegrationTestBase
{
    public RegisterEndpointTests(PostgresFixture fixture) : base(fixture) { }

/// <summary>
/// Запрос для регистрации нового пользователя.
/// </summary>
/// <param name="Name">Имя пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
    private record RegisterRequest(string Name, string Password);

    [Fact]
    public async Task Register_WithValidData_Returns200AndToken()
    {
        var request = new RegisterRequest(Faker.Internet.UserName(), "Password123!");

        var response = await Client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.True(body.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_PersistsUserWithHashedPassword()
    {
        var name = Faker.Internet.UserName();
        var response = await Client.PostAsJsonAsync("/api/users",
            new RegisterRequest(name, "Password123!"));
        response.EnsureSuccessStatusCode();

        var user = Db.Users.SingleOrDefault(u => u.Name == name);
        Assert.NotNull(user);
        Assert.NotEqual("Password123!", user!.Password);
    }

    [Fact]
    public async Task Register_DuplicateName_Fails()
    {
        var name = Faker.Internet.UserName();
        await Client.PostAsJsonAsync("/api/users", new RegisterRequest(name, "Password123!"));

        var response = await Client.PostAsJsonAsync("/api/users",
            new RegisterRequest(name, "AnotherPassword!"));

        Assert.True(response.StatusCode is HttpStatusCode.InternalServerError
                                        or HttpStatusCode.Conflict
                                        or HttpStatusCode.BadRequest);
    }
}