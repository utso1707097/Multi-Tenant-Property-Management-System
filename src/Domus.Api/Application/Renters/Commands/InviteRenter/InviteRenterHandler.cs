using System.Security.Cryptography;
using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Domain.Constants;
using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Renters.Commands.InviteRenter;

public sealed class InviteRenterHandler(
    UserManager<ApplicationUser> userManager,
    IAppDbContext db,
    ITenantProvider tenant,
    TimeProvider clock)
{
    public async Task<Result<InviteRenterResponse>> HandleAsync(
        InviteRenterCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(command.Email);

        if (existing is not null)
        {
            return Result.Failure<InviteRenterResponse>(
                Error.Conflict(
                    "ERR_DUPLICATE_EMAIL",
                    "Email is already registered."));
        }

        if (command.UnitId is not null)
        {
            var unitExists = await db.Units.AnyAsync(
                x => x.Id == command.UnitId,
                cancellationToken);

            if (!unitExists)
            {
                return Result.Failure<InviteRenterResponse>(
                    Error.NotFound("ERR_UNIT_NOT_FOUND", "Unit not found."));
            }
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            FullName = command.FullName,
            TenantId = tenant.TenantId!.Value,
            IsActive = true
        };

        var temporaryPassword =
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)) + "Aa1!";

        var createResult = await userManager.CreateAsync(
            user,
            temporaryPassword);

        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<InviteRenterResponse>(
                Error.Validation(
                    string.Join("; ",
                        createResult.Errors.Select(e => e.Description))));
        }

        var roleResult = await userManager.AddToRoleAsync(
            user,
            DomusRoles.Renter);

        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<InviteRenterResponse>(
                Error.Validation(
                    string.Join("; ",
                        roleResult.Errors.Select(e => e.Description))));
        }

        var inviteToken =
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var inviteExpiresAt =
            clock.GetUtcNow().AddDays(7);

        db.Renters.Add(new Renter
        {
            Id = Guid.NewGuid(),
            OwnerTenantId = tenant.TenantId!.Value,
            UserId = user.Id,
            UnitId = command.UnitId,
            MoveInDate = command.MoveInDate,
            LeaseEndDate = command.LeaseEndDate,
            InviteToken = inviteToken,
            InviteExpires = inviteExpiresAt
        });

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success(
            new InviteRenterResponse(
                user.Id,
                user.Email!,
                inviteToken,
                inviteExpiresAt));
    }
}