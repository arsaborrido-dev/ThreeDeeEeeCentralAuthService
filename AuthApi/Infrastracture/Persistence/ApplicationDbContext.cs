
using AuthApi.Domain;
using AuthApi.Domain.Entitities;
using AuthApi.Domain.Entitities.Portfolio;
using AuthApi.Infrastracture.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection;

namespace AuthApi.Infrastracture.Persistence;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Technology> Technologies { get; set; }
    public DbSet<ProjectScreenshot> ProjectScreenshots { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.ApplyConfiguration(new ContactMessageConfiguration());

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);


        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(
                        nameof(ApplySoftDeleteFilter),
                        BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { builder });
            }
        }


        //builder.Entity<RefreshToken>(entity =>
        //{
        //    entity.HasKey(x => x.Id);

        //    //entity.Property(x => x.Id)
        //    //    .ValueGeneratedNever();

        //    entity.Property(x => x.TokenHash)
        //        .IsRequired()
        //        .HasMaxLength(128);

        //    entity.HasIndex(x => x.TokenHash)
        //        .IsUnique();

        //    entity.Property(x => x.UserId)
        //        .IsRequired();

        //    entity.HasOne(x => x.User)
        //        .WithMany()
        //        .HasForeignKey(x => x.UserId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.Property(x => x.CreatedAtUtc)
        //        .IsRequired();

        //    entity.Property(x => x.ExpiresAtUtc)
        //        .IsRequired();

        //    entity.HasIndex(x => new
        //    {
        //        x.UserId,
        //        x.ExpiresAtUtc
        //    });
        //});

        // Convert List<string> ScreenshotUrls to a single string
        //builder.Entity<Project>()
        //    .Property(p => p.ScreenshotUrls)
        //    .HasConversion(
        //        v => string.Join(";", v),
        //        v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
        //    );

        //builder.Entity<Project>()
        //.HasMany(p => p.Skills)
        //.WithMany(s => s.Projects)
        //.UsingEntity(j => j.ToTable("ProjectSkills"));

        //builder.Entity<Project>()
        //    .HasMany(p => p.Technologies)
        //    .WithMany(t => t.Projects)
        //    .UsingEntity(j => j.ToTable("ProjectTechnologies"));

        //builder.Entity<ProjectScreenshot>()
        //.HasOne(ps => ps.Project)
        //.WithMany(p => p.Screenshots)
        //.HasForeignKey(ps => ps.ProjectId)
        //.OnDelete(DeleteBehavior.Cascade);

        //// Seed Skills
        //builder.Entity<Skill>().HasData(
        //    new Skill { SkillId = 1, Name = "Data Modeling" },
        //    new Skill { SkillId = 2, Name = "ETL Pipelines" },
        //    new Skill { SkillId = 3, Name = "Power BI Dashboarding" },
        //    new Skill { SkillId = 4, Name = "C# Development" },
        //    new Skill { SkillId = 5, Name = "ASP.NET MVC" },
        //    new Skill { SkillId = 6, Name = "SQL Query Optimization" }
        //);

        //// Seed Technologies
        //builder.Entity<Technology>().HasData(
        //    new Technology { TechnologyId = 1, Name = "SQL Server" },
        //    new Technology { TechnologyId = 2, Name = "SSIS" },
        //    new Technology { TechnologyId = 3, Name = "SSRS" },
        //    new Technology { TechnologyId = 4, Name = "Power BI" },
        //    new Technology { TechnologyId = 5, Name = "ASP.NET Core" },
        //    new Technology { TechnologyId = 6, Name = "Entity Framework Core" }
        //);


        //// -------------------------
        //// SEED DATA: PROJECTS
        //// -------------------------
        //builder.Entity<Project>().HasData(
        //    new Project
        //    {
        //        ProjectId = 1,
        //        Title = "Portfolio Website",
        //        ShortDescription = "A personal portfolio built with ASP.NET MVC.",
        //        LongDescription = "Full MVC site with EF Core, Identity, and admin dashboard.",
        //        //TechStack = "C#, MVC, EF Core, SQL Server",
        //        ThumbnailImageUrl = "/images/projects/portfolio-thumb.png",
        //        //ScreenshotUrls = new List<string>(), // EF Core requires empty list
        //        GitHubUrl = "https://github.com/augustus/portfolio",
        //        LiveDemoUrl = "",
        //        CreatedDate = new DateTime(
        //                            2026,
        //                            9,
        //                            15,
        //                            0,
        //                            0,
        //                            0,
        //                            DateTimeKind.Utc),
        //        IsFeatured = true
        //    },
        //    new Project
        //    {
        //        ProjectId = 2,
        //        Title = "Power BI Sales Dashboard",
        //        ShortDescription = "Interactive dashboard for sales analytics.",
        //        LongDescription = "Includes DAX measures, drill-through, and bookmarks.",
        //        //TechStack = "Power BI, DAX, SQL",
        //        ThumbnailImageUrl = "/images/projects/powerbi-thumb.png",
        //        //ScreenshotUrls = new List<string>(),
        //        GitHubUrl = "",
        //        LiveDemoUrl = "",
        //        CreatedDate = new DateTime(
        //                        2026,
        //                        9,
        //                        15,
        //                        0,
        //                        0,
        //                        0,
        //                        DateTimeKind.Utc),
        //        IsFeatured = true
        //    }
        //);
    }

    private static void ApplySoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
    where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}