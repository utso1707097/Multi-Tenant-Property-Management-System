namespace Domus.Application.Common.Interfaces;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    Guid UserId { get; }
}