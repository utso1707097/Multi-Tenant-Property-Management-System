using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Auth.Commands.RevokeToken;

public sealed class RevokeTokenHandler(IAppDbContext db, TimeProvider clock)
{
    public async Task<Result> HandleAsync(
        RevokeTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (storedToken is null)
        {
            // Same response whether missing or already revoked (no leak)
            return Result.Success();
        }

        if (storedToken.Revoked is null && storedToken.Expires > clock.GetUtcNow())
        {
            storedToken.Revoked = clock.GetUtcNow();
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}