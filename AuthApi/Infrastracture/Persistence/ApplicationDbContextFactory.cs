
using AuthApi.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetDirectoryName(
            typeof(ApplicationDbContextFactory).Assembly.Location)
            ?? throw new InvalidOperationException(
                "Could not determine application base path.");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection is not configured.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


//using AuthApi.Infrastracture.Persistence;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;

//namespace AuthApi.Infrastracture.Data
//{
//    public class ApplicationDbContextFactory
//    : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//        public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            var connectionString =
//                Environment.GetEnvironmentVariable(
//                    "MIGRATION_CONNECTIONSTRING");

//            if (string.IsNullOrWhiteSpace(connectionString))
//            {
//                throw new InvalidOperationException(
//                    "MIGRATION_CONNECTIONSTRING is not configured.");
//            }

//            optionsBuilder.UseNpgsql(connectionString);

//            return new ApplicationDbContext(optionsBuilder.Options);
//        }
//    }
//}
