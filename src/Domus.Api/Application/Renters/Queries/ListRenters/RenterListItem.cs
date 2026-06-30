namespace Domus.Application.Renters.Queries.ListRenters;

public sealed record RenterListItem(
    Guid Id,
    string Email,
    string FullName,
    Guid? UnitId,
    DateOnly? MoveInDate,
    DateOnly? LeaseEndDate,
    bool InvitePending,
    DateTimeOffset CreatedAt);