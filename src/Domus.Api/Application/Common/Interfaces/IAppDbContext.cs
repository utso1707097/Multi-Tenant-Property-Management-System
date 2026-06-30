using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Domus.Application.Common.Interfaces;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<BillingSubscription> BillingSubscriptions { get; }
    DbSet<Renter> Renters { get; }
    DbSet<Property> Properties { get; }
    DbSet<Unit> Units { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}