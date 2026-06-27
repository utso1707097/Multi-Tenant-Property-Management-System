using Microsoft.AspNetCore.Identity;

namespace Domus.Infrastructure.Auth;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public Guid? TenantId { get; set; }   // Owner: own Id; Renter: Owner's Id; Admin: null
    public bool IsActive { get; set; } = true;
}