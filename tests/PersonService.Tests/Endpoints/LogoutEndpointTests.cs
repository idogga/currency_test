// Endpoints/LogoutEndpointTests.cs
using System.Net;
using PersonService.Models;
using PersonService.Tests.Fixtures;
using Xunit;

namespace PersonService.Tests.Endpoints;

[Collection(PostgresCollection.Name)]
public class LogoutEndpointTests : IntegrationTestBase
{
    public LogoutEndpointTests(PostgresFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await Client.PostAsync("/api/users/logout", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithValidToken_Returns204()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/logout");
        request.Headers.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());
        var response = await Client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ThenReuseSameToken_IsRejected()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/logout");
        request.Headers.Add(TestAuthHandler.UserIdHeader, Guid.NewGuid().ToString());
        var first = await Client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

        var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/api/users/logout");
        var second = await Client.SendAsync(secondRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
    }

    [Fact]
    public async Task Logout_WithExpiredOrTamperedToken_Returns401()
    {
        AuthorizeWith("not.a.jwt");
        var response = await Client.PostAsync("/api/users/logout", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}