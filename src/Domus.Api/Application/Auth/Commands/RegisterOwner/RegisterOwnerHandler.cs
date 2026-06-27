using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Domain.Constants;
using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Auth.Commands.RegisterOwner;

public sealed class RegisterOwnerHandler(
    UserManager<ApplicationUser> userManager,
    IAppDbContext db)
{
    public async Task<Result<RegisterOwnerResponse>> HandleAsync(
        RegisterOwnerCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(command.Email);
        if (existing is not null)
        {
            return Result.Failure<RegisterOwnerResponse>(
                Error.Conflict("ERR_DUPLICATE_EMAIL", "Email is already registered."));
        }

        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            FullName = command.FullName,
            IsActive = true
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var createResult = await userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<RegisterOwnerResponse>(
                Error.Validation(string.Join("; ", createResult.Errors.Select(e => e.Description))));
        }

        user.TenantId = user.Id;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<RegisterOwnerResponse>(
                Error.Validation(string.Join("; ", updateResult.Errors.Select(e => e.Description))));
        }

        var roleResult = await userManager.AddToRoleAsync(user, DomusRoles.Owner);
        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<RegisterOwnerResponse>(
                Error.Validation(string.Join("; ", roleResult.Errors.Select(e => e.Description))));
        }

        db.BillingSubscriptions.Add(new BillingSubscription
        {
            Id = Guid.NewGuid(),
            OwnerTenantId = user.Id,
            Plan = "FREE",
            RenterLimit = 10
        });

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success(new RegisterOwnerResponse(user.Id, user.Email!, DomusRoles.Owner));
    }
}