using AuthApi.Application.Auth.DTOs;
using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Domain.Entitities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;


//MVC (cookie) -> AuthApi (JWT + Refresh in HttpOnly cookie + Rotation)

namespace AuthApi.Application.Auth;
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _tokenRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        //ApplicationDbContext dbContext,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtOptions,
        IRefreshTokenRepository tokenRepository,            
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        //_dbContext = dbContext;
        _jwtService = jwtService;
        _jwtSettings = jwtOptions.Value;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    public async Task<ResponseResult> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = dto.Email.Trim();

        var existingUser =
            await _userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser != null)
        {
            _logger.LogWarning("[AuthService:RegisterAsync] - Account already exists.");

            return ResponseResult.Failure(
                "EMAIL_ALREADY_EXISTS",
                "An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult =
            await _userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
        {
            _logger.LogWarning("[AuthService:RegisterAsync] - Account creation failed.");

            var errors = createResult.Errors
                .Select(e => e.Description)
                .ToArray();

            return ResponseResult.Failure(
                "REGISTRATION_FAILED",
                string.Join(", ", errors));
        }

        var roleResult =
            await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            _logger.LogWarning("[AuthService:RegisterAsync] - Unable to complete registration.");

            // Compensating action: remove the newly-created user.
            await _userManager.DeleteAsync(user);

            return ResponseResult.Failure(
                "REGISTRATION_FAILED",
                "Unable to complete registration.");
        }

        return ResponseResult.Success();
    }
    public async Task<ResponseResult<ResponseToken>> LoginAsync(
        LoginDto dto,
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            _logger.LogWarning("[AuthService:LoginAsync] - Authentication failed.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_CREDENTIALS",
                "Invalid email or password.");
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordValid)
        {
            _logger.LogWarning("[AuthService:LoginAsync] - Authentication failed: invalid credentials.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_CREDENTIALS",
                "Invalid email or password.");
        }

        var response = await CreateTokenResponseAsync(
            user,
            cancellationToken,
            ipAddress,
            false,
            null);

        return ResponseResult<ResponseToken>.Success(response);
    }

    public async Task<ResponseResult> LogoutAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ResponseResult.Success();
        }

        var token = await _tokenRepository.GetTokenAsync(
            refreshToken,
            cancellationToken);

        if (token == null)
        {
            // Logout should be idempotent.
            // The client is already logged out from our perspective.
            return ResponseResult.Success();
        }

        if (token.RevokedAtUtc != null)
        {
            // Already revoked.
            return ResponseResult.Success();
        }

        await _tokenRepository.RevokeAsync(
            token,
            ipAddress,
            replacementTokenHash: null,
            cancellationToken);

        return ResponseResult.Success();
    }

    public async Task<ResponseResult> CleanupTokensAsync(CancellationToken cancellationToken = default)
    {
        await _tokenRepository.CleanupAsync(cancellationToken);

        return ResponseResult.Success();
    }

    public async Task<ResponseResult<ResponseToken>> RefreshTokenAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token is blank/null.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        var token = await _tokenRepository.GetTokenAsync(
                refreshToken,
                cancellationToken);

        if (token == null)
        {
            _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token was not found.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        //check if either revoked or expired
        if (!token.IsActive)
        {
            if (token.RevokedAtUtc != null)
            {
                _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token was revoked.");

                await _tokenRepository.LogSecurityEvent(
                    token.CreatedByIp,
                    "REVOKED_TOKEN_USED",
                    ipAddress,
                    "Revoked refresh token reuse"
                    );

                // TODO:
                // Evaluate whether the token family/session should be revoked.
            }
            else
            {
                //expired refresh token
                _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token has expired.");
            }

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        //check ip mismatch (possible stolen token)
        // Validate IP address.
        if (!string.Equals(
                token.CreatedByIp,
                ipAddress,
                StringComparison.Ordinal))
        {
            _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token IP address mismatch.");

            await _tokenRepository.LogSecurityEvent(
                    token.CreatedByIp,
                    "IP_ADDRESS_MISMATCHED",
                    ipAddress,
                    "Refresh token IP address mismatch"
                    );

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        // Validate associated user.
        var user = await _userManager.FindByIdAsync(token.UserId);

        if (user == null)
        {
            _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - Refresh token references a non-existent user.");

            await _tokenRepository.LogSecurityEvent(
                    token.CreatedByIp,
                    "INVALID_TOKEN_USER",
                    ipAddress,
                    "Refresh token references a non-existent user"
                    );

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        // Validate account state.
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            _logger.LogWarning(
                "[AuthService:RefreshTokenAsync] - User email is not confirmed.");

            //what happens to refresh token if user state changes???

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        var response = await CreateTokenResponseAsync(
            user,
            cancellationToken,
            ipAddress,
            isRefresh: true,
            token);

        return ResponseResult<ResponseToken>.Success(response);
    }

    public async Task<ResponseResult> RevokeTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("[RefreshTokenService:RevokeTokenAsync] - Refresh token is blank/null.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        var token = await _tokenRepository.GetTokenAsync(
                refreshToken,
                cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("[RefreshTokenService:RevokeTokenAsync] - Refresh token was not found.");

            return ResponseResult<ResponseToken>.Failure(
                "INVALID_REFRESH_TOKEN",
                "Invalid or expired refresh token.");
        }

        await _tokenRepository.RevokeAsync(
            token,
            ipAddress,
            null,
            cancellationToken);

        return ResponseResult.Success();

    }
    private async Task<ResponseToken> CreateTokenResponseAsync(
        ApplicationUser user,
        CancellationToken cancellationToken,
        string? ipAddress,
        bool isRefresh = false,
        RefreshToken? token = null)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireMinutes);

        // Create the new refresh token first.
        var (rawRefreshToken, refreshToken) =
            await _tokenRepository.CreateAsync(
                user,
                ipAddress,
                isRefresh,
                token,
                cancellationToken);

        //revoke previously created token
        if (isRefresh && token != null)
        {
            //revoke old token
            await _tokenRepository.RevokeAsync(
                token,
                ipAddress,
                refreshToken.TokenHash,
                cancellationToken);
        }

        return new ResponseToken(
              accessToken,
              accessTokenExpiresAt,
              rawRefreshToken,
              refreshToken.ExpiresAtUtc);
    }
}


