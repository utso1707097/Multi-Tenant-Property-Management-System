using Domus.Api.Extensions;
using Domus.Infrastructure;
using Domus.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await IdentitySeeder.SeedRolesAsync(app.Services);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();