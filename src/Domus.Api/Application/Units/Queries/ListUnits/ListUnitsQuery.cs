namespace Domus.Application.Units.Queries.ListUnits;

public sealed record ListUnitsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? PropertyId = null);