

using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;

public sealed class SkillConfiguration
    : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {

        builder.HasKey(x => x.SkillId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new Skill
            {
                SkillId = 1,
                Name = "C#",
                Description = "C# programming language"
            },
            new Skill
            {
                SkillId = 2,
                Name = "ASP.NET Core",
                Description = "Cross-platform framework for building web applications"
            },        
            new Skill { SkillId = 3, Name = "Data Modeling" },
            new Skill { SkillId = 4, Name = "ETL Pipelines" },
            new Skill { SkillId = 5, Name = "Power BI Dashboarding" },
            new Skill { SkillId = 6, Name = "C# Development" },
            new Skill { SkillId = 7, Name = "ASP.NET MVC" },
            new Skill { SkillId = 8, Name = "SQL Query Optimization" }
        );
    }
}
