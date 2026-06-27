using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Domus.Api.Tests.Fixtures;
using Domus.Application.Auth;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RegisterOwner;

namespace Domus.Api.Tests.Application.Auth;

public sealed class LoginTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokensWithTenantClaim()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterOwnerCommand(email, "Password123!", "Test Owner", null));

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginCommand(email, "Password123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
        Assert.NotNull(body.TenantId);
        Assert.Equal(body.UserId, body.TenantId);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(body.AccessToken);
        var tenantClaim = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
        Assert.Equal(body.TenantId.ToString(), tenantClaim);
    }
}
