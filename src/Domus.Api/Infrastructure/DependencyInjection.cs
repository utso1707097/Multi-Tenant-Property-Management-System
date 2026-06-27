using System.Text;
using Domus.Application.Auth.Commands.RegisterOwner;
using Domus.Application.Common.Interfaces;
using Domus.Domain.Constants;
using Domus.Infrastructure.Auth;
using Domus.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Domus.Application.Auth.Commands.RefreshToken;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RevokeToken;
using Domus.Application.Auth.Commands.AcceptInvite;

namespace Domus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Name,
                    RoleClaimType = "role"
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("OwnerOnly", p => p.RequireRole(DomusRoles.Owner))
            .AddPolicy("RenterOnly", p => p.RequireRole(DomusRoles.Renter))
            .AddPolicy("SystemAdminOnly", p => p.RequireRole(DomusRoles.SystemAdmin));

        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssemblyContaining<RegisterOwnerValidator>();
        services.AddScoped<RegisterOwnerHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<RefreshTokenHandler>();
        services.AddScoped<RevokeTokenHandler>();
        services.AddScoped<AcceptInviteHandler>();
        return services;
    }
}