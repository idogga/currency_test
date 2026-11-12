using System.Net;
using System.Net.Http.Json;
using PersonService.Application.Dtos;
using PersonService.Common.Services;
using PersonService.Domain.Entities;
using PersonService.Tests.Fixtures;

namespace PersonService.Tests.Endpoints;

/// <summary>
/// Тесты для эндпоинта логина.
/// </summary>
[Collection(PostgresCollection.Name)]
public class LoginEndpointTests : IntegrationTestBase
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LoginEndpointTests"/>.
    /// </summary>
    /// <param name="fixture"></param>
    public LoginEndpointTests(PostgresFixture fixture) : base(fixture) { }

    /// <summary>
    /// Запрос для логина.
    /// </summary>
    /// <param name="Name">Имя пользователя.</param>
    /// <param name="Password">Пароль пользователя.</param>
    private record LoginRequest(string Name, string Password);

    /// <summary>
    /// Создает и сохраняет пользователя в тестовой базе данных, возвращая его имя и пароль.
    /// </summary>
    /// <returns>Имя и пароль пользователя.</returns>
    private async Task<(string Name, string Password)> SeedUserAsync()
    {
        var hasher = new PasswordHasher();
        var name = Faker.Internet.UserName();
        var password = "Password123!";
        Db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Password = hasher.Hash(password)
        });
        await Db.SaveChangesAsync();
        return (name, password);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var (name, password) = await SeedUserAsync();

        var response = await Client.PostAsJsonAsync("/api/users/login", new LoginRequest(name, password));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Fails()
    {
        var (name, _) = await SeedUserAsync();

        var response = await Client.PostAsJsonAsync("/api/users/login",
            new LoginRequest(name, "wrong-password"));

        Assert.True(response.StatusCode is HttpStatusCode.InternalServerError
                                        or HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistingUser_Fails()
    {
        var response = await Client.PostAsJsonAsync("/api/users/login",
            new LoginRequest(Faker.Internet.UserName(), "Password123!"));

        Assert.True(response.StatusCode is HttpStatusCode.InternalServerError
                                        or HttpStatusCode.Unauthorized);
    }
}