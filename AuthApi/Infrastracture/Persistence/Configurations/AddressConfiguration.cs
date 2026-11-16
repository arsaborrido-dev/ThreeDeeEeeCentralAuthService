using Microsoft.EntityFrameworkCore;

using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.AddressId);

        builder.Property(a => a.AddressId)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.ZipCode)
            .IsRequired();

        builder.Property(a => a.AddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(u => u.User)
            .WithMany(a => a.Addresses)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAtUtc)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

    }
}
