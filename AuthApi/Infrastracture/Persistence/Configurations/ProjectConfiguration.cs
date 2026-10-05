
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public sealed class ProjectConfiguration
    : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.ProjectId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ShortDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.LongDescription)
            .IsRequired();

        builder.Property(x => x.ProjectDemonstration)
            .IsRequired();

        builder.Property(x => x.ThumbnailImageUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.GitHubUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.LiveDemoUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(p => p.Skills)
            .WithMany(s => s.Projects)
            .UsingEntity(ps => ps.ToTable("ProjectSkills"));

        builder.HasMany(p => p.Technologies)
            .WithMany(t => t.Projects)
            .UsingEntity(pt => pt.ToTable("ProjectTechnologies"));

        // -------------------------
        // SEED DATA: PROJECTS
        // -------------------------
        builder.HasData(
            new Project
            {
                ProjectId = 1,
                Title = "Portfolio Website",
                ShortDescription = "A personal portfolio built with ASP.NET MVC.",
                LongDescription = "Full MVC site with EF Core, Identity, and admin dashboard.",
                //TechStack = "C#, MVC, EF Core, SQL Server",
                ThumbnailImageUrl = "/images/projects/portfolio-thumb.png",
                //ScreenshotUrls = new List<string>(), // EF Core requires empty list
                GitHubUrl = "https://github.com/augustus/portfolio",
                LiveDemoUrl = "",
                CreatedAtUtc = new DateTime(
                                    2026,
                                    9,
                                    15,
                                    0,
                                    0,
                                    0,
                                    DateTimeKind.Utc),
                IsFeatured = true
            },
            new Project
            {
                ProjectId = 2,
                Title = "Power BI Sales Dashboard",
                ShortDescription = "Interactive dashboard for sales analytics.",
                LongDescription = "Includes DAX measures, drill-through, and bookmarks.",
                //TechStack = "Power BI, DAX, SQL",
                ThumbnailImageUrl = "/images/projects/powerbi-thumb.png",
                //ScreenshotUrls = new List<string>(),
                GitHubUrl = "",
                LiveDemoUrl = "",
                CreatedAtUtc = new DateTime(
                                2026,
                                9,
                                15,
                                0,
                                0,
                                0,
                                DateTimeKind.Utc),
                IsFeatured = true
            }
        );
    }
}
