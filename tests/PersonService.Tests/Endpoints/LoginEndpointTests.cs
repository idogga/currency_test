using System.Net;
using System.Net.Http.Json;
using PersonService.Common.Services;
using PersonService.Dtos;
using PersonService.Models;
using PersonService.Tests;
using PersonService.Tests.Fixtures;

namespace PersonService.Tests.Endpoints;

[Collection(PostgresCollection.Name)]
public class LoginEndpointTests : IntegrationTestBase
{
    public LoginEndpointTests(PostgresFixture fixture) : base(fixture) { }

    private record LoginRequest(string Name, string Password);

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