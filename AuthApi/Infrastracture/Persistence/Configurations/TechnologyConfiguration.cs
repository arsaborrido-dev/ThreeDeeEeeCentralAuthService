
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public sealed class TechnologyConfiguration
    : IEntityTypeConfiguration<Technology>
{
    public void Configure(EntityTypeBuilder<Technology> builder)
    {
        builder.ToTable("Technologies");

        builder.HasKey(x => x.TechnologyId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasOne(s => s.TechnologyCategory)
            .WithMany(c => c.Technologies)
            .HasForeignKey(s => s.TechnologyCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Technologies
        builder.HasData(

            new Technology { TechnologyId = 1, Name = "SQL Server", TechnologyCategoryId = 3, Description = "" },
            //new Technology { TechnologyId = 2, Name = "Docker" },
            new Technology { TechnologyId = 3, Name = "Visual Studio", TechnologyCategoryId = 3, Description = "" },
            new Technology { TechnologyId = 4, Name = "Power BI", TechnologyCategoryId = 3, Description = "" },
            new Technology { TechnologyId = 5, Name = "GitHub", TechnologyCategoryId = 3, Description = "" },
            //new Technology { TechnologyId = 6, Name = "Entity Framework Core" }
            new Technology { TechnologyId = 6, Name = "Team Foundation Server", TechnologyCategoryId = 3, Description = "" },
            new Technology { TechnologyId = 7, Name = "OnContact 8.0 CRM", TechnologyCategoryId = 3, Description = "" },

            new Technology
            {
                TechnologyId = 8,
                TechnologyCategoryId = 1,
                Name = "ASP.NET Core MVC",
                Description = ""
            },

            new Technology
            {
                TechnologyId = 9,
                TechnologyCategoryId = 1,
                Name = "Razor",
                Description = ""
            },

            new Technology
            {
                TechnologyId = 10,
                TechnologyCategoryId = 1,
                Name = "HTML",
                Description = ""
            },

            new Technology
            {
                TechnologyId = 11,
                TechnologyCategoryId = 1,
                Name = "CSS",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 12,
                TechnologyCategoryId = 1,
                Name = "JavaScript",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 13,
                TechnologyCategoryId = 2,
                Name = "C#",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 14,
                TechnologyCategoryId = 2,
                Name = "ASP.NET Core Web API",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 15,
                TechnologyCategoryId = 2,
                Name = "Typed HttpClient",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 16,
                TechnologyCategoryId = 3,
                Name = "PostgreSQL",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 17,
                TechnologyCategoryId = 3,
                Name = "Entity Framework Core",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 18,
                TechnologyCategoryId = 4,
                Name = "ASP.NET Core Identity",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 19,
                TechnologyCategoryId = 4,
                Name = "JWT",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 20,
                TechnologyCategoryId = 4,
                Name = "Cloudflare Turnstile",
                Description = ""
            }, 
            new Technology
            {
                TechnologyId = 21,
                TechnologyCategoryId = 5,
                Name = "Docker",
                Description = ""
            },
            new Technology
            {
                TechnologyId = 22,
                TechnologyCategoryId = 5,
                Name = "Docker Compose",
                Description = ""
            });

//        Tools
//Docker — Containerizing applications and services

//SSMS — Managing SQL Server databases

//Visual Studio — Main IDE for .NET development

//GitHub — Version control and project hosting

//Team Foundation Server — Source control and work item tracking

//Azure Data Studio — Lightweight SQL development and management

//Postman — Testing and debugging APIs

//OnContact CRM Tools — Supporting CRM‑related development tasks
    }
}
