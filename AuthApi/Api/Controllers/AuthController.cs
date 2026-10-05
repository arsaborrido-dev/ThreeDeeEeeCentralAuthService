
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

    //user logout
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
