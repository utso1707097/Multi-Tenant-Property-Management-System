using Domus.Domain.Common;
using Domus.Infrastructure.Auth;

namespace Domus.Domain.Entities;

public sealed class Renter : ITenantScoped
{
    public Guid Id { get; init; }
    public Guid OwnerTenantId { get; set; }
    public Guid UserId { get; set; }
    public string? InviteToken { get; set; }
    public DateTimeOffset? InviteExpires { get; set; }
    public Guid? UnitId { get; set; }
    public DateOnly? MoveInDate { get; set; }
    public DateOnly? LeaseEndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Unit? Unit { get; set; }
    public ApplicationUser User { get; set; } = null!;
}