using Domus.Domain.Enums;

namespace Domus.Application.Units.Commands.CreateUnit;

public sealed record CreateUnitResponse(
    Guid Id,
    Guid PropertyId,
    string UnitNumber,
    short? Floor,
    short? Bedrooms,
    decimal RentAmount,
    string Currency,
    UnitStatus Status,
    DateTimeOffset CreatedAt);