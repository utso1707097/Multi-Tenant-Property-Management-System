namespace Domus.Application.Properties.Queries.ListProperties;

public sealed record PropertyListItem(
    Guid Id,
    string Name,
    string AddressLine,
    string City,
    string Country,
    DateTimeOffset CreatedAt);