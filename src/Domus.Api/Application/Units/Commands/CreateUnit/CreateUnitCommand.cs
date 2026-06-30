namespace Domus.Application.Units.Commands.CreateUnit;

public sealed record CreateUnitCommand(
    Guid PropertyId,
    string UnitNumber,
    short? Floor,
    short? Bedrooms,
    decimal RentAmount,
    string Currency);