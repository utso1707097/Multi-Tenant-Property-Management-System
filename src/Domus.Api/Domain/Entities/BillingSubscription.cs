namespace Domus.Domain.Entities;

public sealed class BillingSubscription
{
    public Guid Id { get; init; }
    public Guid OwnerTenantId { get; set; }
    public string Plan { get; set; } = "FREE";
    public int RenterLimit { get; set; } = 10;
}