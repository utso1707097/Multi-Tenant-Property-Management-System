using Domus.Application.Common.Interfaces;
using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Domus.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options),
      IAppDbContext
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BillingSubscription> BillingSubscriptions => Set<BillingSubscription>();
    public DbSet<Renter> Renters => Set<Renter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}