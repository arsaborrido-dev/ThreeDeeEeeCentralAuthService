

using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;

public sealed class TechnologyConfiguration
    : IEntityTypeConfiguration<Technology>
{
    public void Configure(EntityTypeBuilder<Technology> builder)
    {

        builder.HasKey(x => x.TechnologyId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Seed Technologies
        builder.HasData(
            new Technology { TechnologyId = 1, Name = "SQL Server" },
            new Technology { TechnologyId = 2, Name = "SSIS" },
            new Technology { TechnologyId = 3, Name = "SSRS" },
            new Technology { TechnologyId = 4, Name = "Power BI" },
            new Technology { TechnologyId = 5, Name = "ASP.NET Core" },
            new Technology { TechnologyId = 6, Name = "Entity Framework Core" }
        );
    }
}
