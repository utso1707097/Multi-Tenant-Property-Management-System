using Domus.Domain.Entities;
using Domus.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domus.Infrastructure.Persistence.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.OwnerTenantId);
        builder.HasIndex(x => new { x.PropertyId, x.UnitNumber }).IsUnique();
        builder.Property(x => x.UnitNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Floor).IsRequired(false);
        builder.Property(x => x.Bedrooms).IsRequired(false);
        builder.Property(x => x.RentAmount).HasPrecision(12, 2).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();
        builder.HasOne(x => x.Property)
        .WithMany()
        .HasForeignKey(x => x.PropertyId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}