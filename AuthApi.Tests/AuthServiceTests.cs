
using AuthApi.Application.Auth;
using AuthApi.Application.Auth.DTOs;
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

    private const string Email = "user@example.com";
    private const string Password = "StrongPassword123!";
    private const string IpAddress = "192.0.2.10";
    private const string RefreshTokenValue = "test-refresh-token";

    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IRefreshTokenRepository> _tokenRepository = new();
    private readonly Mock<ILogger<AuthService>> _logger = new();

    private readonly AuthService _authService;


    public AuthServiceTests()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        var settings = new JwtSettings
        {
            Key = "A-Long-Test-Key-That-Is-At-Least-32-Bytes!",
            Issuer = "AuthApi",
            Audience = "MvcPortFApp",
            AccessTokenExpireMinutes = 15
        };

        _authService = new AuthService(
            _userManager.Object,
            _jwtService.Object,
            Options.Create(settings),
            _tokenRepository.Object,
            _logger.Object);
    }

    private static RegisterDto CreateRegisterDto() => new()
    {
        Email = Email,
        FirstName = "Test",
        LastName = "User",
        Password = Password
    };

    private static LoginDto CreateLoginDto() => new()
    {
        Email = Email,
        Password = Password
    };

    private static ApplicationUser CreateUser() => new()
    {
        Id = "user-123",
        UserName = Email,
        Email = Email,
        FirstName = "Test",
        LastName = "User"
    };

    private static RefreshToken CreateActiveToken(
        string userId = "user-123",
        string ipAddress = IpAddress)
    {
        return new RefreshToken
        {
            UserId = userId,
            CreatedByIp = ipAddress,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
    }

    private void SetupSuccessfulTokenCreation(
        ApplicationUser user,
        RefreshToken? previousToken = null)
    {
        _userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _jwtService
            .Setup(x => x.GenerateAccessToken(
                It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .Returns("test-access-token");

        _tokenRepository
            .Setup(x => x.CreateAsync(
                user,
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<RefreshToken?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                "new-raw-refresh-token",
                new RefreshToken
                {
                    UserId = user.Id,
                    CreatedByIp = IpAddress,
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
                }));
    }

    // REGISTER

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_DoesNotCreateUser()
    {
        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync(CreateUser());

        var result = await _authService.RegisterAsync(
            CreateRegisterDto(), CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _userManager.Verify(
            x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserCreationFails_ReturnsFailure()
    {
        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                Password))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Password is invalid." }));

        var result = await _authService.RegisterAsync(
            CreateRegisterDto(), CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _userManager.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenRoleAssignmentFails_DeletesCreatedUser()
    {
        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManager
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Failed());

        _userManager
            .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(
            CreateRegisterDto(), CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _userManager.Verify(
            x => x.DeleteAsync(It.IsAny<ApplicationUser>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationSucceeds_AddsUserRole()
    {
        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManager
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(
            CreateRegisterDto(), CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _userManager.Verify(
            x => x.AddToRoleAsync(
                It.Is<ApplicationUser>(u =>
                    u.Email == Email &&
                    u.UserName == Email),
                "User"),
            Times.Once);
    }

    // LOGIN

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ReturnsFailure()
    {
        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(
            CreateLoginDto(), IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _userManager.Verify(
            x => x.CheckPasswordAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ReturnsFailure()
    {
        var user = CreateUser();

        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.CheckPasswordAsync(user, Password))
            .ReturnsAsync(false);

        var result = await _authService.LoginAsync(
            CreateLoginDto(), IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<RefreshToken?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_CreatesTokens()
    {
        var user = CreateUser();

        _userManager
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.CheckPasswordAsync(user, Password))
            .ReturnsAsync(true);

        SetupSuccessfulTokenCreation(user);

        var result = await _authService.LoginAsync(
            CreateLoginDto(), IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _jwtService.Verify(
            x => x.GenerateAccessToken(
                It.IsAny<IEnumerable<System.Security.Claims.Claim>>()),
            Times.Once);

        _tokenRepository.Verify(
            x => x.CreateAsync(
                user,
                IpAddress,
                false,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // LOGOUT

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task LogoutAsync_WhenRefreshTokenIsBlank_DoesNotQueryRepository(
        string refreshToken)
    {
        var result = await _authService.LogoutAsync(
            refreshToken, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenDoesNotExist_DoesNotRevoke()
    {
        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _authService.LogoutAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.RevokeAsync(
                It.IsAny<RefreshToken>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenIsActive_RevokesIt()
    {
        var token = CreateActiveToken();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _authService.LogoutAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.RevokeAsync(
                token,
                IpAddress,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task LogoutAsync_WhenTokenIsRevoked_DoesNotRevoke()
    {
        var token = CreateActiveToken();
        token.RevokedAtUtc = DateTime.UtcNow;

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _authService.LogoutAsync(
           RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
          x => x.RevokeAsync(
              It.IsAny<RefreshToken>(),
              It.IsAny<string?>(),
              It.IsAny<string?>(),
              It.IsAny<CancellationToken>()),
          Times.Never);
    }

    // CLEANUP

    [Fact]
    public async Task CleanupTokensAsync_CallsRepositoryCleanup()
    {
        var result = await _authService.CleanupTokensAsync();

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.CleanupAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // REFRESH TOKEN

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RefreshTokenAsync_WhenTokenIsBlank_ReturnsFailure(
        string refreshToken)
    {
        var result = await _authService.RefreshTokenAsync(
            refreshToken, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenTokenDoesNotExist_ReturnsFailure()
    {
        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

    }

    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsRevoked_LogsSecurityEvent()
    {
        var token = CreateActiveToken();
        token.RevokedAtUtc = DateTime.UtcNow;

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.LogSecurityEvent(
                token.CreatedByIp,
                "REVOKED_TOKEN_USED",
                IpAddress,
                "Revoked refresh token reuse"),
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenIpAddressDiffers_LogsSecurityEvent()
    {
        var token = CreateActiveToken();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue,
            "192.0.2.99",
            CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.LogSecurityEvent(
                token.CreatedByIp,
                "IP_ADDRESS_MISMATCHED",
                "192.0.2.99",
                "Refresh token IP address mismatch"),
            Times.Once);

        _userManager.Verify(
            x => x.FindByIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenUserDoesNotExist_LogsSecurityEvent()
    {
        var token = CreateActiveToken();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userManager
            .Setup(x => x.FindByIdAsync(token.UserId))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.LogSecurityEvent(
                token.CreatedByIp,
                "INVALID_TOKEN_USER",
                IpAddress,
                "Refresh token references a non-existent user"),
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenEmailIsNotConfirmed_DoesNotCreateTokens()
    {
        var token = CreateActiveToken();
        var user = CreateUser();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userManager
            .Setup(x => x.FindByIdAsync(token.UserId))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(false);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<RefreshToken?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenTokenAndUserAreValid_CreatesNewTokens()
    {
        var token = CreateActiveToken();
        var user = CreateUser();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userManager
            .Setup(x => x.FindByIdAsync(token.UserId))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        SetupSuccessfulTokenCreation(user, token);

        var result = await _authService.RefreshTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.CreateAsync(
                user,
                IpAddress,
                true,
                token,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // REVOKE

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RevokeTokenAsync_WhenTokenIsBlank_DoesNotQueryRepository(
        string refreshToken)
    {
        var result = await _authService.RevokeTokenAsync(
            refreshToken, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenDoesNotExist_DoesNotRevoke()
    {
        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _authService.RevokeTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.False(result.Succeeded);

        _tokenRepository.Verify(
            x => x.RevokeAsync(
                It.IsAny<RefreshToken>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenExists_RevokesIt()
    {
        var token = CreateActiveToken();

        _tokenRepository
            .Setup(x => x.GetTokenAsync(
                RefreshTokenValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _authService.RevokeTokenAsync(
            RefreshTokenValue, IpAddress, CancellationToken.None);

        //Assert.NotNull(result);
        Assert.True(result.Succeeded);

        _tokenRepository.Verify(
            x => x.RevokeAsync(
                token,
                IpAddress,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    
}