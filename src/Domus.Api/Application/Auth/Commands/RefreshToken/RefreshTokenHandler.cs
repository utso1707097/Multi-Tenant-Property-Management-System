using Domus.Application.Auth;
using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = Domus.Domain.Entities.RefreshToken;

namespace Domus.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IAppDbContext db,
    IOptions<JwtSettings> jwtOptions,
    TimeProvider clock)
{
    public async Task<Result<AuthResponse>> HandleAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (storedToken is null)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized("ERR_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token."));
        }

        // Reuse detection: token was already rotated/revoked
        if (storedToken.Revoked is not null)
        {
            await RevokeAllUserTokensAsync(storedToken.UserId, cancellationToken);
            return Result.Failure<AuthResponse>(
                Error.Unauthorized("ERR_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token."));
        }

        // Expired but never revoked
        if (storedToken.Expires <= clock.GetUtcNow())
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized("ERR_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token."));
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
        {
            return Result.Failure<AuthResponse>(
                Error.Unauthorized("ERR_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthResponse>(
                Error.Forbidden("ERR_ACCOUNT_INACTIVE", "Account is inactive."));
        }

        // Rotate
        var newRefreshValue = tokenService.CreateRefreshToken();
        var newRefreshExpiresAt = clock.GetUtcNow().AddDays(jwtOptions.Value.RefreshTokenDays);
        var now = clock.GetUtcNow();

        storedToken.Revoked = now;
        storedToken.ReplacedByToken = newRefreshValue;

        db.RefreshTokens.Add(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            Token = newRefreshValue,
            UserId = user.Id,
            Created = now,
            Expires = newRefreshExpiresAt
        });

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = tokenService.CreateAccessToken(user, roles);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(
            user.Id,
            user.Email!,
            roles.ToList(),
            user.TenantId,
            accessToken,
            accessExpiresAt,
            newRefreshValue,
            newRefreshExpiresAt));
    }

    private async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.Revoked == null)
            .ToListAsync(cancellationToken);

        var now = clock.GetUtcNow();
        foreach (var token in activeTokens)
            token.Revoked = now;

        await db.SaveChangesAsync(cancellationToken);
    }
}