namespace Domus.Application.Auth;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    IReadOnlyList<string> Roles,
    Guid? TenantId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);
