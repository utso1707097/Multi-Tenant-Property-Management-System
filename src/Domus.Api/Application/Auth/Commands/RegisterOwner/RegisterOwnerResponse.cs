namespace Domus.Application.Auth.Commands.RegisterOwner;

public sealed record RegisterOwnerResponse(
    Guid Id,
    string Email,
    string Role);