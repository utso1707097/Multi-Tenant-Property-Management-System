using Domus.Application.Common.Interfaces;
using Domus.Domain.Entities;

namespace Domus.Application.Properties.Commands.CreateProperty;

public sealed class CreatePropertyHandler(
    IAppDbContext db,
    ITenantProvider tenant,
    TimeProvider clock)
{
    public async Task<Result<CreatePropertyResponse>> HandleAsync(
        CreatePropertyCommand command,
        CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();

        var property = new Property
        {
            Id = Guid.NewGuid(),
            OwnerTenantId = tenant.TenantId!.Value,
            Name = command.Name,
            AddressLine = command.AddressLine,
            City = command.City,
            Country = command.Country,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Properties.Add(property);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new CreatePropertyResponse(
                property.Id,
                property.Name,
                property.AddressLine,
                property.City,
                property.Country,
                property.CreatedAt));
    }
}