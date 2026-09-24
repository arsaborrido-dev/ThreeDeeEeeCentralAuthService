
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastracture.Persistence.Configurations;
public sealed class ProjectScreenshotConfiguration
    : IEntityTypeConfiguration<ProjectScreenshot>
{
    public void Configure(EntityTypeBuilder<ProjectScreenshot> builder)
    {
        builder.HasKey(x => x.ProjectScreenshotId);

        builder.Property(x => x.ProjectScreenshotId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.Caption)
            .HasMaxLength(500);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder
            .HasOne(x => x.Project)
            .WithMany(p => p.Screenshots)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.ProjectId, x.DisplayOrder })
            .IsUnique();
    }
}