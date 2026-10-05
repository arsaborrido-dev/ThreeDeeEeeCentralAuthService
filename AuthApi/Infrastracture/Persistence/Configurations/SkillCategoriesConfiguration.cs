

using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public class SkillCategoriesConfiguration : IEntityTypeConfiguration<SkillCategory>
{
    public void Configure(EntityTypeBuilder<SkillCategory> builder)
    {
        builder.ToTable("SkillCategories");

        builder.HasKey(c => c.SkillCategoryId);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
           .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasData(
            new SkillCategory
            {
                SkillCategoryId = 1,
                Name = "Backend & Application Development",
                Description = "Technologies used to build backend services, web applications, and internal tools."
            },
            new SkillCategory
            {
                SkillCategoryId = 2,
                Name = "Data & Integrations",
                Description = "Tools and processes for moving, transforming, and integrating data across systems."
            },
            new SkillCategory
            {
                SkillCategoryId = 3,
                Name = "Reporting & Analytics",
                Description = "Tools used to present data and build operational reports."
            },
            new SkillCategory
            {
                SkillCategoryId = 4,
                Name = "Others",
                Description = "General skills that support development, maintenance, and operational stability."
            });

    }
}
