using Domus.Application.Common.Interfaces;
using Domus.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Domus.Api.Tests.Fixtures;

public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestJwtKey =
        "integration-test-jwt-signing-key-min-32-chars";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16")
        .Build();

    public HttpClient Client => CreateClient();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _postgres.GetConnectionString(),
                ["JwtSettings:Key"] = TestJwtKey,
                ["JwtSettings:Issuer"] = "https://api.domus.app",
                ["JwtSettings:Audience"] = "domus-api",
                ["JwtSettings:AccessTokenMinutes"] = "15",
                ["JwtSettings:RefreshTokenDays"] = "7",
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Migrate before the host runs Program (IdentitySeeder needs schema)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        await using (var db = new AppDbContext(options, new NullTenantProvider()))
            await db.Database.MigrateAsync();

        _ = CreateClient();
    }

    private sealed class NullTenantProvider : ITenantProvider
    {
        public Guid? TenantId => null;

        public Guid UserId => Guid.Empty;
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}
