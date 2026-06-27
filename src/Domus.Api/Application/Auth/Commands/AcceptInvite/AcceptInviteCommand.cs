namespace Domus.Application.Auth.Commands.AcceptInvite;

public sealed record AcceptInviteCommand(string InviteToken, string Password);