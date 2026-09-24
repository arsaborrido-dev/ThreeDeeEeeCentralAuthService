using AuthApi.Api.Controllers;
using AuthApi.Application.Auth.DTOs;
using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Domain.Entitities;
using AuthApi.Infrastracture.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;


//MVC (cookie) -> AuthApi (JWT + Refresh in HttpOnly cookie + Rotation)

namespace AuthApi.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtOptions,
            IRefreshTokenService refreshTokenService,            
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtService = jwtService;
            _jwtSettings = jwtOptions.Value;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }
        
        //public async Task<AuthResult> RefreshAsync(
        public async Task<ResponseResult<ResponseToken>> RefreshAsync(
            string refreshToken, 
            string ipAddress, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("[AuthService:RefreshAsync] - Invalid refresh token.");

                return ResponseResult<ResponseToken>.Failure(
                    "INVALID_REFRESH_TOKEN",
                    "Invalid or expired refresh token.");
            }

            var token = await _refreshTokenService.GetActiveAsync(
                    refreshToken,
                    cancellationToken);          

            if (token == null)
            {
                _logger.LogWarning("[AuthService:RefreshAsync] - Refresh token is null.");

                return ResponseResult<ResponseToken>.Failure(
                    "INVALID_REFRESH_TOKEN",
                    "Invalid or expired refresh token.");
            }

            if (!token.IsActive)
            {
                _logger.LogWarning("[AuthService:RefreshAsync] - Refresh token is inactive.");

                return ResponseResult<ResponseToken>.Failure(
                    "INVALID_REFRESH_TOKEN",
                    "Invalid or expired refresh token.");
            }
                       
            var user = await _userManager.FindByIdAsync(token.UserId);

            if (user == null)
            {
                _logger.LogWarning("[AuthService:RefreshAsync] - Token user is null.");

                return ResponseResult<ResponseToken>.Failure(
                    "INVALID_REFRESH_TOKEN",
                    "Invalid or expired refresh token.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("[AuthService:RefreshAsync] - User email is not confirmed.");

                return ResponseResult<ResponseToken>.Failure(
                    "INVALID_REFRESH_TOKEN",
                    "Invalid or expired refresh token.");
            }

            var response = await CreateTokenResponseAsync(
                user,
                cancellationToken,
                ipAddress, 
                true, 
                token);

            //if (response == null)
            //{
            //    _logger.LogWarning("[AuthService:RefreshAsync] - Created token is null.");

            //    return ResponseResult<ResponseToken>.Failure(
            //        "INVALID_REFRESH_TOKEN",
            //        "Invalid or expired refresh token.");
            //}

            return ResponseResult<ResponseToken>.Success(response);
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

            //if (response == null)
            //{
            //    _logger.LogWarning("[AuthService:LoginAsync] - Created token is null.");

            //    return ResponseResult<ResponseToken>.Failure(
            //        "INVALID_CREDENTIALS",
            //        "Invalid email or password.");
            //}

            return ResponseResult<ResponseToken>.Success(response);
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

        public async Task RevokeAsync(
            string refreshToken,
            string ipAddress,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("[AuthService:RevokeAsync] - Invalid refresh token.");

                return;
            }                

            var token = await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(t => t.TokenHash == refreshToken);

            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("[AuthService:RevokeAsync] - Invalid or inactive refresh token.");

                return;
            }

            token.RevokedAtUtc = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;

            await _dbContext.SaveChangesAsync();
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
                await _refreshTokenService.CreateAsync(
                    user.Id,
                    ipAddress,
                    cancellationToken);

            user.RefreshTokens.Add(refreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (isRefresh && token != null)
            {
                //revoke old token
                await _refreshTokenService.RevokeAsync(
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
}

//////public async Task CleanupRefreshTokensAsync()
//////{
//////    var cutoff = DateTime.UtcNow.AddDays(-30);

//////    var tokens = _dbContext.RefreshTokens
//////        .Where(t => t.IsRevoked && t.RevokedAt < cutoff || t.ExpiresAt < DateTime.UtcNow);

//////    _dbContext.RefreshTokens.RemoveRange(tokens);

//////    await _dbContext.SaveChangesAsync();
//////}


////What your refresh endpoint should do when a revoked token is used
////1. Log the event
////Store:

////IP

////timestamp

////user ID

////token ID

////2. Invalidate all tokens for that user
////Force logout everywhere.

////3. Alert the user (optional)
////“Suspicious login attempt detected.”

////4. Block the request
////Return:

////csharp
////return Unauthorized("Refresh token has been revoked.");
///

////public class SecurityEvent
////{
////    public Guid Id { get; set; }
////    public Guid UserId { get; set; }
////    public string EventType { get; set; } // e.g., "RevokedTokenUsed"
////    public string IpAddress { get; set; }
////    public DateTime Timestamp { get; set; }
////    public string Details { get; set; }
////}

////_dbContext.SecurityEvents.Add(new SecurityEvent
////{
////    UserId = token.UserId,
////    EventType = "RevokedTokenUsed",
////    IpAddress = currentIp,
////    Timestamp = DateTime.UtcNow,
////    Details = "Refresh token was revoked but attempted again."
////});