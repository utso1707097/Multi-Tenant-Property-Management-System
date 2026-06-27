namespace Domus.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; init; }
    public string Token { get; set; } = "";
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Expires { get; set; }
    public DateTimeOffset? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => Revoked is null && DateTimeOffset.UtcNow < Expires;
}