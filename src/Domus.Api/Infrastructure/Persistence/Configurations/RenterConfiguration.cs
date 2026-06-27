using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domus.Infrastructure.Persistence.Configurations;

public sealed class RenterConfiguration : IEntityTypeConfiguration<Renter>
{
    public void Configure(EntityTypeBuilder<Renter> builder)
    {
        builder.ToTable("renters");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.InviteToken).IsUnique();
        builder.HasIndex(x => x.OwnerTenantId);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}