
using AuthApi.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthApi.Tests.Integration;

public sealed class PostgreSqlTestFixtureTests(
    PostgreSqlTestFixture fixture)
    : IClassFixture<PostgreSqlTestFixture>
{
    [Fact]
    public async Task ContainerStarts_AndAllMigrationsAreApplied()
    {
        await using var context = fixture.CreateDbContext();

        var canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect);

        var appliedMigrations =
            await context.Database.GetAppliedMigrationsAsync();

        var expectedMigrations =
            context.Database.GetMigrations();

        Assert.Equal(expectedMigrations, appliedMigrations);

        Assert.Contains(
            "20261108084637_correct-entity-timestamp-defaults-11082026",
            appliedMigrations);
    }
}
