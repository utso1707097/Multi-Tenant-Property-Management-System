namespace Domus.Application.Auth.Commands.RegisterOwner;

public sealed record RegisterOwnerCommand(
    string Email,
    string Password,
    string FullName,
    string? CompanyName);