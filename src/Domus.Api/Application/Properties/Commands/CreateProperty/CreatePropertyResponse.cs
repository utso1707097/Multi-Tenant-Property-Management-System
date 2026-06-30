namespace Domus.Application.Properties.Commands.CreateProperty;

public sealed record CreatePropertyResponse(
    Guid Id,
    string Name,
    string AddressLine,
    string City,
    string Country,
    DateTimeOffset CreatedAt);