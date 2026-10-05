

using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public sealed class SkillConfiguration
    : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {

        builder.ToTable("Skills");

        builder.HasKey(x => x.SkillId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasOne(s => s.SkillCategory)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.SkillCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Skill
            {
                SkillId = 1,
                SkillCategoryId = 1,
                Name = "C# Development",
                Description = "Primary language for backend and application logic"
            },

            new Skill
            {
                SkillId = 2,
                SkillCategoryId = 1,
                Name = "ASP.NET Core",
                Description = "Cross‑platform framework for building web applications"
            },
            new Skill
            {
                SkillId = 3,
                SkillCategoryId = 1,
                Name = "ASP.NET MVC",
                Description = "Model‑View‑Controller framework for structured web applications"
            },
            new Skill
            {
                SkillId = 4,
                SkillCategoryId = 1,
                Name = "ASP.NET Web API",
                Description = "Framework for building RESTful APIs"
            },
            new Skill
            {
                SkillId = 5,
                SkillCategoryId = 1,
                Name = "Entity Framework",
                Description = "ORM for managing data access and database interactions in .NET"
            },
            new Skill
            {
                SkillId = 6,
                SkillCategoryId = 1,
                Name = "ASP.NET WebForms",
                Description = "" //"Experience maintaining and supporting legacy web applications"
            },
            new Skill
            {
                SkillId = 7,
                SkillCategoryId = 1,
                Name = "VB.Net / VB6",
                Description = ""
            },
            new Skill
            {
                SkillId = 8,
                SkillCategoryId = 1,
                Name = "Windows Forms",
                Description = ""
            },
            new Skill
            {
                SkillId = 9,
                SkillCategoryId = 1,
                Name = "Dockerized .NET Applications",
                Description = "Running .NET services inside containers for portability"
            },
            new Skill
            {
                SkillId = 10,
                SkillCategoryId = 2,
                Name = "SSIS",
                Description = "Packages, automation, and script tasks for ETL workflows"
            },
            new Skill
            {
                SkillId = 11,
                SkillCategoryId = 2,
                Name = "ETL Pipelines",
                Description = "Extract, transform, and load processes for structured data"
            },
            new Skill
            {
                SkillId = 12,
                SkillCategoryId = 2,
                Name = "Data Migration & Transformation",
                Description = "Moving and shaping data between systems"
            },
            new Skill
            {
                SkillId = 13,
                SkillCategoryId = 2,
                Name = "API Integrations",
                Description = "Connecting applications and services through APIs"
            },
            new Skill
            {
                SkillId = 14,
                SkillCategoryId = 2,
                Name = "PostgreSQL Integrations",
                Description = "Using PostgreSQL for containerized backend services"
            },
            new Skill
            {
                SkillId = 15,
                SkillCategoryId = 3,
                Name = "SSRS",
                Description = "Datasets, expressions, and drill‑down reporting"
            },
            new Skill
            {
                SkillId = 16,
                SkillCategoryId = 3,
                Name = "Power BI",
                Description = "Query Optimization for Reporting"
            },
            new Skill
            {
                SkillId = 17,
                SkillCategoryId = 4,
                Name = "Data Validation",
                Description = "Ensuring accuracy and consistency of data"
            },
            new Skill
            {
                SkillId = 18,
                SkillCategoryId = 4,
                Name = "Troubleshooting",
                Description = "Identifying and resolving issues across systems"
            },
            new Skill
            {
                SkillId = 19,
                SkillCategoryId = 4,
                Name = "Documentation",
                Description = "Writing clear and organized technical notes"
            },
            new Skill
            {
                SkillId = 20,
                SkillCategoryId = 4,
                Name = "Performance Tuning",
                Description = "Improving application and database performance"
            },
            new Skill
            {
                SkillId = 21,
                SkillCategoryId = 4,
                Name = "Team Foundation Server",
                Description = "Work item tracking and source control"
            },
            new Skill
            {
                SkillId = 22,
                SkillCategoryId = 4,
                Name = "Version Control (Git)",
                Description = "Managing source code and project history"
            },
            new Skill
            {
                SkillId = 23,
                SkillCategoryId = 4,
                Name = "OnContact CRM Development",
                Description = "Supporting CRM‑related development tasks"
            }
            //new Skill { SkillId = 3, Name = "Data Modeling" },
            //new Skill { SkillId = 4, Name = "ETL Pipelines" },
            //new Skill { SkillId = 5, Name = "Power BI Dashboarding" },
            //new Skill { SkillId = 6, Name = "C# Development" },
            //new Skill { SkillId = 7, Name = "ASP.NET MVC" },
            //new Skill { SkillId = 8, Name = "SQL Query Optimization" }
        );
    }
}
