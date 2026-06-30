using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Results;
using Domus.Domain.Entities;
using Domus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Units.Commands.CreateUnit;

public sealed class CreateUnitHandler(
    IAppDbContext db,
    ITenantProvider tenant,
    TimeProvider clock)
{
    public async Task<Result<CreateUnitResponse>> HandleAsync(
        CreateUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var propertyExists = await db.Properties
            .AnyAsync(p => p.Id == command.PropertyId, cancellationToken);

        if (!propertyExists)
        {
            return Result.Failure<CreateUnitResponse>(
                Error.NotFound("ERR_NOT_FOUND", "Property not found."));
        }

        var duplicate = await db.Units.AnyAsync(
            u => u.PropertyId == command.PropertyId &&
                 u.UnitNumber == command.UnitNumber,
            cancellationToken);

        if (duplicate)
        {
            return Result.Failure<CreateUnitResponse>(
                Error.Conflict(
                    "ERR_DUPLICATE_UNIT",
                    "A unit with this number already exists."));
        }

        var now = clock.GetUtcNow();

        var unit = new Unit
        {
            Id = Guid.NewGuid(),
            OwnerTenantId = tenant.TenantId!.Value,
            PropertyId = command.PropertyId,
            UnitNumber = command.UnitNumber,
            Floor = command.Floor,
            Bedrooms = command.Bedrooms,
            RentAmount = command.RentAmount,
            Currency = command.Currency,
            Status = UnitStatus.Vacant,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Units.Add(unit);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new CreateUnitResponse(
                unit.Id,
                unit.PropertyId,
                unit.UnitNumber,
                unit.Floor,
                unit.Bedrooms,
                unit.RentAmount,
                unit.Currency,
                unit.Status,
                unit.CreatedAt));
    }
}