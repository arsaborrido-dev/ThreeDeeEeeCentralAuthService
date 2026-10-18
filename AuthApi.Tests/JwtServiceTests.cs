using AuthApi.Application.Common.Models;
using AuthApi.Infrastracture.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Tests;

public class JwtServiceTests
{
    private const string SigningKey =
        "ThisIsATestSigningKeyAtLeast32BytesLong!";

    private const string Issuer = "AuthApi";
    private const string Audience = "MvcPortFApp";

    private static JwtSettings CreateNullKeySettings(
        string? key = null,
        string? issuer = null,
        string? audience = null,
        int expirationMinutes = 15)
    {
        return new JwtSettings
        {
            Key = key,
            Issuer = issuer ?? Issuer,
            Audience = audience ?? Audience,
            AccessTokenExpireMinutes = expirationMinutes
        };
    }

    private static JwtSettings CreateSettings(
        string? key = null,
        string? issuer = null,
        string? audience = null,
        int expirationMinutes = 15)
    {
        return new JwtSettings
        {
            Key = key ?? SigningKey,
            Issuer = issuer ?? Issuer,
            Audience = audience ?? Audience,
            AccessTokenExpireMinutes = expirationMinutes
        };
    }

    private static JwtService CreateService(
        JwtSettings? settings = null)
    {
        return new JwtService(
            Options.Create(settings ?? CreateSettings()));
    }

    // 1. A valid token should be generated.
    [Fact]
    public void GenerateAccessToken_ReturnsNonEmptyToken()
    {
        var service = CreateService();

        var token = service.GenerateAccessToken(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        ]);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    // 2. The generated token should contain the expected issuer,
    // audience, signing algorithm, and expiration.
    [Fact]
    public void GenerateAccessToken_ContainsExpectedTokenMetadata()
    {
        var service = CreateService();

        var tokenString = service.GenerateAccessToken(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        ]);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        Assert.Equal(Issuer, token.Issuer);
        Assert.Contains(Audience, token.Audiences);
        Assert.Equal(
            SecurityAlgorithms.HmacSha256,
            token.Header.Alg);

        Assert.NotNull(token.ValidFrom);
        Assert.NotNull(token.ValidTo);

        Assert.True(token.ValidTo > token.ValidFrom);
        Assert.True(
            token.ValidTo <= DateTime.UtcNow.AddMinutes(16));
    }

    // 3. Custom claims should be included in the token.
    [Fact]
    public void GenerateAccessToken_IncludesProvidedClaims()
    {
        var service = CreateService();

        var tokenString = service.GenerateAccessToken(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-123"),
            new Claim("role", "Admin"),
            new Claim("custom_claim", "test-value")
        ]);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(tokenString);

        Assert.Contains(
            token.Claims,
            c => c.Type == ClaimTypes.NameIdentifier
                 && c.Value == "user-123");

        Assert.Contains(
            token.Claims,
            c => c.Type == "role"
                 && c.Value == "Admin");

        Assert.Contains(
            token.Claims,
            c => c.Type == "custom_claim"
                 && c.Value == "test-value");
    }

    // 4. A unique JTI should be generated even if the caller
    // provides an existing JTI claim.
    [Fact]
    public void GenerateAccessToken_ReplacesExistingJti()
    {
        var service = CreateService();

        var tokenString = service.GenerateAccessToken(
        [
            new Claim(JwtRegisteredClaimNames.Jti, "old-jti")
        ]);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(tokenString);

        var jtiClaims = token.Claims
            .Where(c => c.Type == JwtRegisteredClaimNames.Jti)
            .ToList();

        Assert.Single(jtiClaims);
        Assert.NotEqual("old-jti", jtiClaims[0].Value);

        Assert.True(
            Guid.TryParseExact(jtiClaims[0].Value, "N", out _));
    }

    // 5. The generated token should contain an issued-at claim.
    [Fact]
    public void GenerateAccessToken_IncludesIssuedAtClaim()
    {
        var service = CreateService();

        var tokenString = service.GenerateAccessToken(
        [
            new Claim("custom_claim", "test")
        ]);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(tokenString);

        var issuedAt = Assert.Single(
            token.Claims.Where(
                c => c.Type == JwtRegisteredClaimNames.Iat));

        Assert.True(long.TryParse(issuedAt.Value, out var timestamp));

        var issuedAtUtc = DateTimeOffset
            .FromUnixTimeSeconds(timestamp)
            .UtcDateTime;

        Assert.InRange(
            issuedAtUtc,
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(1));
    }

    // 6. A valid, correctly signed token should validate.
    [Fact]
    public void ValidateAccessToken_ValidToken_ReturnsPrincipal()
    {
        var service = CreateService();

        var token = service.GenerateAccessToken(
        [
            new Claim("custom_claim", "test-value")
        ]);

        var principal = service.ValidateAccessToken(token);

        Assert.NotNull(principal);
        Assert.Contains(
            principal.Claims,
            c => c.Type == "custom_claim"
                 && c.Value == "test-value");
    }

    // 7. A null, empty, or whitespace token should be rejected.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateAccessToken_EmptyToken_ReturnsNull(
        string? token)
    {
        var service = CreateService();

        var principal = service.ValidateAccessToken(token!);

        Assert.Null(principal);
    }

    // 8. Malformed tokens should be rejected.
    [Theory]
    [InlineData("not-a-jwt")]
    [InlineData("header.payload.signature")]
    public void ValidateAccessToken_MalformedToken_ReturnsNull(
        string token)
    {
        var service = CreateService();

        var principal = service.ValidateAccessToken(token);

        Assert.Null(principal);
    }

    // 9. A token signed using another key should be rejected.
    [Fact]
    public void ValidateAccessToken_WrongSigningKey_ReturnsNull()
    {
        var service = CreateService();

        var token = CreateSignedToken(
            key: "ACompletelyDifferentSigningKey32BytesLong!",
            issuer: Issuer,
            audience: Audience,
            expires: DateTime.UtcNow.AddMinutes(5));

        Assert.Null(service.ValidateAccessToken(token));
    }

    // 10. A token with a different issuer should be rejected.
    [Fact]
    public void ValidateAccessToken_WrongIssuer_ReturnsNull()
    {
        var service = CreateService();

        var token = CreateSignedToken(
            key: SigningKey,
            issuer: "WrongIssuer",
            audience: Audience,
            expires: DateTime.UtcNow.AddMinutes(5));

        Assert.Null(service.ValidateAccessToken(token));
    }

    // 11. A token with a different audience should be rejected.
    [Fact]
    public void ValidateAccessToken_WrongAudience_ReturnsNull()
    {
        var service = CreateService();

        var token = CreateSignedToken(
            key: SigningKey,
            issuer: Issuer,
            audience: "WrongAudience",
            expires: DateTime.UtcNow.AddMinutes(5));

        Assert.Null(service.ValidateAccessToken(token));
    }

    // 12. An expired token outside the configured clock skew
    // should be rejected.
    [Fact]
    public void ValidateAccessToken_ExpiredToken_ReturnsNull()
    {
        var service = CreateService();

        var token = CreateSignedToken(
            key: SigningKey,
            issuer: Issuer,
            audience: Audience,
            expires: DateTime.UtcNow.AddMinutes(-5));

        Assert.Null(service.ValidateAccessToken(token));
    }

    // 13. Tampering with the payload should invalidate its signature.
    [Fact]
    public void ValidateAccessToken_TamperedToken_ReturnsNull()
    {
        var service = CreateService();

        var token = service.GenerateAccessToken(
        [
            new Claim("custom_claim", "original-value")
        ]);

        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);

        // Change the encoded payload while retaining the original
        // signature. The token should no longer validate.
        parts[1] = parts[1].Length > 1
            ? (parts[1][0] == 'A' ? "B" : "A") + parts[1][1..]
            : "A";

        var tamperedToken = string.Join(".", parts);

        Assert.Null(service.ValidateAccessToken(tamperedToken));
    }

    // 14. A missing signing key should prevent service construction.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_MissingSigningKey_Throws(
        string? key)
    {
        var settings = CreateNullKeySettings(key: key);

        Assert.Throws<InvalidOperationException>(
            () => CreateService(settings));
    }

    // 15. A signing key shorter than 32 UTF-8 bytes should be rejected.
    [Fact]
    public void Constructor_ShortSigningKey_Throws()
    {
        var settings = CreateSettings(key: "short-key");

        var exception = Assert.Throws<InvalidOperationException>(
            () => CreateService(settings));

        Assert.Contains(
            "at least 32 bytes",
            exception.Message);
    }

    // Helper for generating tokens that deliberately violate
    // issuer, audience, signing-key, or lifetime expectations.
    private static string CreateSignedToken(
        string key,
        string issuer,
        string audience,
        DateTime expires)
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims:
            [
                new Claim("custom_claim", "test-value")
            ],
            notBefore: now.AddMinutes(-10),
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}