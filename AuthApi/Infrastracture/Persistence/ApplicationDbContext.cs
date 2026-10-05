
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
    //public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SecurityEvent> SecurityEvents { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Technology> Technologies { get; set; }
    public DbSet<ProjectScreenshot> ProjectScreenshots { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //apply all entity configurations
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
    }
    private static void ApplySoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
    where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}