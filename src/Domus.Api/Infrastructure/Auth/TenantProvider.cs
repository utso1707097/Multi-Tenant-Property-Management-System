using System.Security.Claims;
using Domus.Application.Common.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Domus.Infrastructure.Auth;

public sealed class TenantProvider(IHttpContextAccessor accessor) : ITenantProvider
{
    public Guid? TenantId
    {
        get
        {
            var claim = accessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public Guid UserId =>
        Guid.Parse(accessor.HttpContext!.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
}