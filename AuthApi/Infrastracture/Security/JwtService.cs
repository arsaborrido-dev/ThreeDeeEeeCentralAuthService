using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;

namespace AuthApi.Infrastracture.Security;
public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    private readonly SymmetricSecurityKey _signingKey;
    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.Key))
            throw new InvalidOperationException("JWT signing key is not configured.");

        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);

        // Require at least 256 bits / 32 bytes for HS256.
        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must be at least 32 bytes long.");
        }

        _signingKey = new SymmetricSecurityKey(keyBytes);
    }
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var now = DateTime.UtcNow;

        var allClaims = claims
            .Where(c => !string.Equals(
                c.Type,
                JwtRegisteredClaimNames.Jti,
                StringComparison.Ordinal))
            .ToList();

        allClaims.Add(
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString("N")));

        allClaims.Add(
            new Claim(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64));

        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: allClaims,
            notBefore: now,
            expires: now.AddMinutes(
                _settings.AccessTokenExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,

            ValidateAudience = true,
            ValidAudience = _settings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,

            ValidateLifetime = true,

            // Don't silently allow long-expired tokens.
            ClockSkew = TimeSpan.FromSeconds(30),

            // Explicitly allow only HS256.
            ValidAlgorithms = new[]
            {
                SecurityAlgorithms.HmacSha256
            },

            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();

            return handler.ValidateToken(
                token,
                validationParameters,
                out _);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}

