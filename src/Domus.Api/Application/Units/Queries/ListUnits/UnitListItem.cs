namespace Domus.Application.Units.Queries.ListUnits;

public sealed record UnitListItem(
    Guid Id,
    Guid PropertyId,
    string UnitNumber,
    short? Floor,
    short? Bedrooms,
    decimal RentAmount,
    string Currency,
    Domain.Enums.UnitStatus Status,
    DateTimeOffset CreatedAt);