using System.Net;
using System.Net.Http.Json;
using Domus.Api.Tests.Fixtures;
using Domus.Application.Auth.Commands.RegisterOwner;
using Domus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Domus.Api.Tests.Application.Auth;

public sealed class RegisterOwnerTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task RegisterOwner_ValidRequest_CreatesUserAndBilling()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        var request = new RegisterOwnerCommand(email, "Password123!", "Test Owner", null);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RegisterOwnerResponse>();
        Assert.NotNull(body);
        Assert.Equal(email, body.Email);
        Assert.Equal("Owner", body.Role);

        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var billing = await db.BillingSubscriptions
            .FirstOrDefaultAsync(b => b.OwnerTenantId == body.Id);

        Assert.NotNull(billing);
        Assert.Equal("FREE", billing.Plan);
        Assert.Equal(10, billing.RenterLimit);
    }
}
