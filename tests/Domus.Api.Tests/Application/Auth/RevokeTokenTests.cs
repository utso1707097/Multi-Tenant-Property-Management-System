using System.Net;
using System.Net.Http.Json;
using Domus.Api.Tests.Fixtures;
using Domus.Application.Auth;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RefreshToken;
using Domus.Application.Auth.Commands.RegisterOwner;
using Domus.Application.Auth.Commands.RevokeToken;

namespace Domus.Api.Tests.Application.Auth;

public sealed class RevokeTokenTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Revoke_ActiveToken_PreventsRefresh()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterOwnerCommand(email, "Password123!", "Test Owner", null));

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginCommand(email, "Password123!"));
        var loginBody = await login.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginBody);

        var revoke = await _client.PostAsJsonAsync("/api/v1/auth/revoke",
            new RevokeTokenCommand(loginBody.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, revoke.StatusCode);

        var refresh = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(loginBody.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }
}
