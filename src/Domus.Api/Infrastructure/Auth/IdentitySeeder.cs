using Domus.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Domus.Infrastructure.Auth;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles =
        [
            DomusRoles.Owner,
            DomusRoles.Renter,
            DomusRoles.SystemAdmin
        ];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
}