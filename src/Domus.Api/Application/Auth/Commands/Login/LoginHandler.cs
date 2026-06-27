using Domus.Application.Auth;
using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Infrastructure.Auth;
using RefreshTokenEntity = Domus.Domain.Entities.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Domus.Application.Auth.Commands.Login;

public sealed class LoginHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IAppDbContext db,
    IOptions<JwtSettings> jwtOptions,
    TimeProvider clock)
{
    public async Task<Result<AuthResponse>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, command.Password))
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized("ERR_INVALID_CREDENTIALS", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthResponse>(
                Error.Forbidden("ERR_ACCOUNT_INACTIVE", "Account is inactive."));
        }

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = tokenService.CreateAccessToken(user, roles);
        var refreshTokenValue = tokenService.CreateRefreshToken();
        var refreshExpiresAt = clock.GetUtcNow().AddDays(jwtOptions.Value.RefreshTokenDays);

        db.RefreshTokens.Add(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            Created = clock.GetUtcNow(),
            Expires = refreshExpiresAt
        });

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(
            user.Id,
            user.Email!,
            roles.ToList(),
            user.TenantId,
            accessToken,
            accessExpiresAt,
            refreshTokenValue,
            refreshExpiresAt));
    }
}