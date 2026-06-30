namespace Domus.Application.Renters.Commands.InviteRenter;

public sealed record InviteRenterResponse(
    Guid RenterId,
    string Email,
    string InviteToken,
    DateTimeOffset InviteExpiresAt);