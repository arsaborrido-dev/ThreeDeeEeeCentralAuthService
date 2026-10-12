
//using AuthApi.Domain.Entitities;

//namespace AuthApi.Tests;

//public class RefreshTokenTests
//{
//    [Fact]
//    public void IsActive_WhenTokenHasNotExpiredAndIsNotRevoked_ReturnsTrue()
//    {
//        // Arrange
//        var token = new RefreshToken
//        {
//            CreatedAtUtc = DateTimeOffset.UtcNow.AddMinutes(-5),
//            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5),
//            RevokedAtUtc = null
//        };

//        // Act
//        var isActive = token.IsActive;

//        // Assert
//        Assert.True(isActive);
//    }

//    [Fact]
//    public void IsActive_WhenTokenHasExpired_ReturnsFalse()
//    {
//        // Arrange
//        var token = new RefreshToken
//        {
//            CreatedAtUtc = DateTimeOffset.UtcNow.AddMinutes(-10),
//            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(-5),
//            RevokedAtUtc = null
//        };

//        // Act
//        var isActive = token.IsActive;

//        // Assert
//        Assert.False(isActive);
//    }

//    [Fact]
//    public void IsActive_WhenTokenHasBeenRevoked_ReturnsFalse()
//    {
//        // Arrange
//        var now = DateTimeOffset.UtcNow;

//        var token = new RefreshToken
//        {
//            CreatedAtUtc = now.AddMinutes(-5),
//            ExpiresAtUtc = now.AddMinutes(5),
//            RevokedAtUtc = now.AddMinutes(-1)
//        };

//        // Act
//        var isActive = token.IsActive;

//        // Assert
//        Assert.False(isActive);
//    }
//}


using AuthApi.Application.Auth;
using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Domain.Entitities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthApi.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsBlank_ReturnsInvalidRefreshToken()
    {
        // Arrange
        var userStore = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var jwtService = new Mock<IJwtService>();
        var tokenRepository = new Mock<IRefreshTokenRepository>();
        var logger = Mock.Of<ILogger<AuthService>>();

        var jwtOptions = Options.Create(new JwtSettings());

        var authService = new AuthService(
            userManager.Object,
            jwtService.Object,
            jwtOptions,
            tokenRepository.Object,
            logger);

        // Act
        var result = await authService.RefreshTokenAsync(
            string.Empty,
            "127.0.0.1");

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("INVALID_REFRESH_TOKEN", result.ErrorCode);

        tokenRepository.Verify(
            repository => repository.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}