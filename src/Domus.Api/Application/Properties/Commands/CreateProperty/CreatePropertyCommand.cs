namespace Domus.Application.Properties.Commands.CreateProperty;

public sealed record CreatePropertyCommand(
    string Name,
    string AddressLine,
    string City,
    string Country);