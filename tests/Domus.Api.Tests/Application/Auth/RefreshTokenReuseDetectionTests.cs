using System.Net;
using System.Net.Http.Json;
using Domus.Api.Tests.Fixtures;
using Domus.Application.Auth;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RefreshToken;
using Domus.Application.Auth.Commands.RegisterOwner;

namespace Domus.Api.Tests.Application.Auth;

public sealed class RefreshTokenReuseDetectionTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Refresh_ReusesRevokedToken_RevokesAllTokens()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterOwnerCommand(email, "Password123!", "Test Owner", null));

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginCommand(email, "Password123!"));
        var loginBody = await login.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginBody);

        var tokenA = loginBody.RefreshToken;

        var refresh1 = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(tokenA));
        var body1 = await refresh1.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body1);

        var tokenB = body1.RefreshToken;

        var reuse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(tokenA));
        Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);

        var afterReuse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshTokenCommand(tokenB));
        Assert.Equal(HttpStatusCode.Unauthorized, afterReuse.StatusCode);
    }
}
