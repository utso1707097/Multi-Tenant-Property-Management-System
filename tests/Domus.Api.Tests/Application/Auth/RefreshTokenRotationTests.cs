using System.Net;
using System.Net.Http.Json;
using Domus.Api.Tests.Fixtures;
using Domus.Application.Auth;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RefreshToken;
using Domus.Application.Auth.Commands.RegisterOwner;

namespace Domus.Api.Tests.Application.Auth;

public sealed class RefreshTokenRotationTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewPairAndRotates()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterOwnerCommand(email, "Password123!", "Test Owner", null));

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginCommand(email, "Password123!"));
        var loginBody = await login.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginBody);

        var oldRefresh = loginBody.RefreshToken;

        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(oldRefresh));

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var newBody = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(newBody);
        Assert.NotEqual(oldRefresh, newBody.RefreshToken);

        var reuseResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(oldRefresh));

        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);
    }
}
