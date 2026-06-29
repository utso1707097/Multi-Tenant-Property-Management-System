namespace Domus.Domain.Common;

public interface ITenantScoped
{
    Guid OwnerTenantId { get; set; }
}