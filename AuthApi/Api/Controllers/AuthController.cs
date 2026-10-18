
using AuthApi.Application.Auth;
using AuthApi.Application.Auth.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using AuthApi.Application.Common.Models;

//using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
//using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;

namespace AuthApi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly RefreshTokenSettings _tokenSettings;
    public AuthController(IAuthService authService, 
        IWebHostEnvironment environment,
        ILogger<AuthController> logger,
        IOptions<RefreshTokenSettings> tokenOptions)
    {
        _authService = authService;
        _environment = environment;
        _logger = logger;
        _tokenSettings = tokenOptions.Value;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new user using AuthService.RegisterAsync.
    /// Returns 201 on success, or 409 if registration fails.
    /// </remarks>
    /// <param name="dto">User registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="201">Registration successful</response>
    /// <response code="409">Registration failed</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]    
    public async Task<IActionResult> Register(
        RegisterDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "[AuthController:Register] - Registration failed: {ErrorMessage}",
                result.ErrorMessage);

            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Registration failed",
                detail: _environment.IsDevelopment()
                    ? result.ErrorMessage
                    : null);            
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new { message = "Registration successful" }
            );
    }


    /// <summary>
    /// Logs user.
    /// </summary>
    /// <remarks>
    /// This endpoint logs a user using AuthService.LoginAsync.
    /// Returns 200 on success, or 401 if login fails.
    /// </remarks>
    /// <param name="dto">User login data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Login successful</response>
    /// <response code="401">Login failed</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]    
    public async Task<IActionResult> Login(
        LoginDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            dto,
            GetIpAddress(),
            cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
              "[AuthController:Login] - Authentication failed for {Email}: {ErrorMessage}.",
              dto.Email,
              result.ErrorMessage);

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid email or password.",
                detail: _environment.IsDevelopment()
                    ? result.ErrorMessage
                    : null);
        }

        var response = result.Data!;

        //create cookie
        SetRefreshTokenCookie(response.RefreshToken);

        return Ok(new
        {
            accessToken = response.AccessToken,
            expiresAt = response.AccessTokenExpiresAtUtc
        });
    }

    /// <summary>
    /// Logs user out.
    /// </summary>
    /// <remarks>
    /// This endpoint logs out a user using AuthService.LogoutAsync.
    /// Returns 200 on success.
    /// </remarks>
    /// <param name="request">User logout request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Logout successful</response>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _authService.LogoutAsync(
            request.RefreshToken,
            ipAddress ?? string.Empty,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Refreshes token.
    /// </summary>
    /// <remarks>
    /// This endpoint refreshes token from HttpOnly cookie using AuthService.RefreshTokenAsync.
    /// Returns 200 on success, or 401 if login fails.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Refresh token successful</response>
    /// <response code="401">Refresh token failed</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("[AuthController:Refresh] - Invalid refresh request.");

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid refresh request.",
                detail: _environment.IsDevelopment()
                    ? "refreshToken does not exists."
                    : null);
        }

        var result = await _authService.RefreshTokenAsync(
            refreshToken,
            GetIpAddress(),
            cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
              "[AuthController:Refresh] - Invalid refresh request: {ErrorMessage}",
              result.ErrorMessage);

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid refresh request.",
                detail: _environment.IsDevelopment()
                    ? result.ErrorMessage
                    : null);
        }

        var response = result.Data;

        return Ok(response);
    }

    /// <summary>
    /// Revokes token.
    /// </summary>
    /// <remarks>
    /// This endpoint revokes token from HttpOnly cookie using AuthService.RevokeTokenAsync.
    /// Returns 200 on success, or 401 if login fails.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Refresh token successful</response>
    [HttpPost("revoke")]
    [AllowAnonymous]
    [EnableRateLimiting("auth-strict")]
    public async Task<IActionResult> Revoke(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("[AuthController:Revoke] - Refresh token is blank or null.");

            DeleteRefreshTokenCookie();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        await _authService.RevokeTokenAsync(
                refreshToken,
                GetIpAddress(),
                cancellationToken);

        DeleteRefreshTokenCookie();

        //return NoContent();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// 'who am I?' endpoint. 
    /// </summary>
    /// <remarks>
    /// This is a standard authenticated /me endpoint.
    /// Returns the identity of the currently logged‑in user based on their JWT claims.
    /// </remarks>    
    /// <response code="200">Successful</response>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<CurrentUserDto> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        var roles = User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        return Ok(new CurrentUserDto(
            id,
            email,
            roles));
    }
    private void SetRefreshTokenCookie(
        string token)
    {
        Response.Cookies.Append(
            "refreshToken",
            token,
            new CookieOptions
            {
                HttpOnly = false, //true,
                Secure = false, //true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_tokenSettings.RefreshTokenExpireDays),
                Path = _tokenSettings.CookiePath //"/api/auth"
            });
    }
    private void DeleteRefreshTokenCookie()
    {     
        Response.Cookies.Delete(
            "refreshToken", 
            new CookieOptions
            {
                Path = _tokenSettings.CookiePath //"/api/auth"
            });

    }
    private string GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?
            .MapToIPv4()
            .ToString()
            ?? "unknown";
    }
}
