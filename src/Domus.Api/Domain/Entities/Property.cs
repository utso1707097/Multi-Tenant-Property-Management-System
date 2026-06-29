using Domus.Domain.Common;

namespace Domus.Domain.Entities;

public sealed class Property : ITenantScoped
{
    public Guid Id { get; init; }
    public Guid OwnerTenantId { get; set; }
    public string Name { get; set; } = "";
    public string AddressLine { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";   // ISO 3166-1 alpha-2, e.g. "US"
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}