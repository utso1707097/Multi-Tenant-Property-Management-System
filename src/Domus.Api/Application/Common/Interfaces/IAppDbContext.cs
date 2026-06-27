using Domus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<BillingSubscription> BillingSubscriptions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}