using AuthApi.Domain.Entitities;

namespace AuthApi.Tests;

public class RefreshTokenTests
{
    [Fact]
    public void IsActive_WhenTokenHasNotExpiredAndIsNotRevoked_ReturnsTrue()
    {
        var token = new RefreshToken
        {
            CreatedAtUtc = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            RevokedAtUtc = null
        };

        Assert.True(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenTokenHasExpired_ReturnsFalse()
    {
        var token = new RefreshToken
        {
            CreatedAtUtc = DateTimeOffset.UtcNow.AddMinutes(-10),
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(-5),
            RevokedAtUtc = null
        };

        Assert.False(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenTokenHasBeenRevoked_ReturnsFalse()
    {
        var now = DateTimeOffset.UtcNow;

        var token = new RefreshToken
        {
            CreatedAtUtc = now.AddMinutes(-5),
            ExpiresAtUtc = now.AddMinutes(5),
            RevokedAtUtc = now.AddMinutes(-1)
        };

        Assert.False(token.IsActive);
    }
}
