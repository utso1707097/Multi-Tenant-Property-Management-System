namespace Domus.Domain.Entities;

public sealed class Renter
{
    public Guid Id { get; init; }
    public Guid OwnerTenantId { get; set; }
    public Guid UserId { get; set; }
    public string? InviteToken { get; set; }
    public DateTimeOffset? InviteExpires { get; set; }
}