using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.HasKey(p => p.PhoneId);

        builder.Property(p => p.PhoneId)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.AreaCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAtUtc)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
