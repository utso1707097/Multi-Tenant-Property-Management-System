namespace Domus.Application.Renters.Commands.InviteRenter;

public sealed record InviteRenterCommand(
    string Email,
    string FullName,
    Guid? UnitId,
    DateOnly? MoveInDate,
    DateOnly? LeaseEndDate);