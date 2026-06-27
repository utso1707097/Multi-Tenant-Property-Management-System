namespace Domus.Application.Auth.Commands.AcceptInvite;

public sealed record AcceptInviteResponse(Guid Id, string Email, string Role);