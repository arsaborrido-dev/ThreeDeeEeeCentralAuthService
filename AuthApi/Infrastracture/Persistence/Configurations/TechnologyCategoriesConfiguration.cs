using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public class TechnologyCategoriesConfiguration : IEntityTypeConfiguration<TechnologyCategory>
{
    public void Configure(EntityTypeBuilder<TechnologyCategory> builder)
    {
        builder.ToTable("TechnologyCategories");

        builder.HasKey(c => c.TechnologyCategoryId);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
           .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasData(
            new TechnologyCategory
            {
                TechnologyCategoryId = 1,
                Name = "Frontend",
                Description = ""
            },
            new TechnologyCategory
            {
                TechnologyCategoryId = 2,
                Name = "Backend",
                Description = ""
            },
            new TechnologyCategory
            {
                TechnologyCategoryId = 3,
                Name = "Data",
                Description = ""
            },
            new TechnologyCategory
            {
                TechnologyCategoryId = 4,
                Name = "Security",
                Description = ""
            },
            new TechnologyCategory
            {
                TechnologyCategoryId = 5,
                Name = "Infrastructure",
                Description = ""
            });
    }
}
