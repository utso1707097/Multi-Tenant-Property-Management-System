using Domus.Domain.Common;
using Domus.Domain.Enums;

namespace Domus.Domain.Entities;

public sealed class Unit : ITenantScoped
{
    public Guid Id { get; init; }
    public Guid OwnerTenantId { get; set; }
    public Guid PropertyId { get; set; }
    public string UnitNumber { get; set; } = "";
    public short? Floor { get; set; }
    public short? Bedrooms { get; set; }
    public decimal RentAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public UnitStatus Status { get; set; } = UnitStatus.Vacant;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Property Property { get; set; } = null!;
}