using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Domus.Domain.Entities;


namespace Domus.Application.Common.Interfaces;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<BillingSubscription> BillingSubscriptions { get; }
    DbSet<Renter> Renters { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbSet<Property> Properties { get; }
    DbSet<Unit> Units { get; }
}