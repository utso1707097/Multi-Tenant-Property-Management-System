using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Domain.Constants;
using Domus.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Auth.Commands.AcceptInvite;

public sealed class AcceptInviteHandler(
    UserManager<ApplicationUser> userManager,
    IAppDbContext db,
    TimeProvider clock)
{
    public async Task<Result<AcceptInviteResponse>> HandleAsync(
        AcceptInviteCommand command,
        CancellationToken cancellationToken = default)
    {
        var renter = await db.Renters
            .FirstOrDefaultAsync(r => r.InviteToken == command.InviteToken, cancellationToken);

        if (renter is null)
        {
            return Result.Failure<AcceptInviteResponse>(
            new Error("ERR_INVALID_INVITE_TOKEN", "Invalid invite token.", StatusCodes.Status400BadRequest));
            // Maps to ERR_INVALID_INVITE_TOKEN via code — or use:
            // new Error("ERR_INVALID_INVITE_TOKEN", "Invalid invite token.", 400)
        }

        if (renter.InviteExpires is null || renter.InviteExpires <= clock.GetUtcNow())
        {
            return Result.Failure<AcceptInviteResponse>(
                new Error("ERR_INVITE_EXPIRED", "Invite has expired.", StatusCodes.Status410Gone));
        }

        var user = await userManager.FindByIdAsync(renter.UserId.ToString());
        if (user is null)
        {
            return Result.Failure<AcceptInviteResponse>(
                new Error("ERR_INVALID_INVITE_TOKEN", "Invalid invite token.", StatusCodes.Status400BadRequest));
        }

        // User was created at invite with a random password — replace it
        if (await userManager.HasPasswordAsync(user))
        {
            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return Result.Failure<AcceptInviteResponse>(
                    Error.Validation(string.Join("; ", removeResult.Errors.Select(e => e.Description))));
            }
        }

        var addResult = await userManager.AddPasswordAsync(user, command.Password);
        if (!addResult.Succeeded)
        {
            return Result.Failure<AcceptInviteResponse>(
                Error.Validation(string.Join("; ", addResult.Errors.Select(e => e.Description))));
        }

        renter.InviteToken = null;
        renter.InviteExpires = null;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AcceptInviteResponse(user.Id, user.Email!, DomusRoles.Renter));
    }
}