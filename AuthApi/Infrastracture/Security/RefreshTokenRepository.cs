using AuthApi.Application.Auth;
using AuthApi.Application.Common.Models;
using AuthApi.Domain.Entitities;
using AuthApi.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthApi.Infrastracture.Security;
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;
    //private readonly JwtSettings _jwtSettings;
    private readonly RefreshTokenSettings _tokenSettings;
    public RefreshTokenRepository(
        ApplicationDbContext context, 
        IOptions<RefreshTokenSettings> tokenOptions)
    {
        _context = context;
        _tokenSettings = tokenOptions.Value;
    }

    public async Task<(string RawToken, RefreshToken Entity)> CreateAsync(
        ApplicationUser user,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateToken();

        var now = DateTime.UtcNow;

        var entity = new RefreshToken
        {
            TokenHash = HashToken(rawToken),
            UserId = user.Id,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_tokenSettings.RefreshTokenExpireDays),
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(entity);
        user.RefreshTokens.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return (rawToken, entity);
    }

    public async Task<(string RawToken, RefreshToken Entity)?> RotateAsync(
        ApplicationUser user,
        RefreshToken currentToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var rawToken = GenerateToken();

        var newToken = new RefreshToken
        {
            TokenHash = HashToken(rawToken),
            UserId = user.Id,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_tokenSettings.RefreshTokenExpireDays),
            CreatedByIp = ipAddress
        };

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        var affectedRows = await _context.RefreshTokens
            .Where(t =>
                t.Id == currentToken.Id &&
                t.RevokedAtUtc == null &&
                t.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.RevokedAtUtc, now)
                .SetProperty(t => t.RevokedByIp, ipAddress)
                .SetProperty(t => t.ReplacedByTokenHash, newToken.TokenHash),
                cancellationToken);

        if (affectedRows != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return null;
        }

        _context.RefreshTokens.Add(newToken);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return (rawToken, newToken);
    }

    //public async Task<(string RawToken, RefreshToken Entity)> CreateAsync(
    //    //string userId,
    //    ApplicationUser user,
    //    string? ipAddress,
    //    bool isRefresh = false,
    //    RefreshToken? token = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    var rawToken = GenerateToken();

    //    var entity = new RefreshToken
    //    {
    //        //Id = Guid.NewGuid(),
    //        TokenHash = HashToken(rawToken),
    //        UserId = user.Id,
    //        CreatedAtUtc = DateTime.UtcNow,
    //        ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireDays),
    //        CreatedByIp = ipAddress
    //    };

    //    _context.RefreshTokens.Add(entity);

    //    //add token to user
    //    user.RefreshTokens.Add(entity);

    //    //revoke previously created token
    //    if (isRefresh && token != null)
    //    {
    //        ////revoke old token
    //        token.RevokedAtUtc = DateTime.UtcNow;
    //        token.RevokedByIp = ipAddress;
    //        token.ReplacedByTokenHash = entity.ReplacedByTokenHash;
    //    }

    //    await _context.SaveChangesAsync(cancellationToken);

    //    return (rawToken, entity);
    //}
    public async Task<RefreshToken?> GetTokenAsync(
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return null;

        var hash = HashToken(rawToken);

        return await _context.RefreshTokens
            .SingleOrDefaultAsync(
                x => x.TokenHash == hash,
                cancellationToken);
    }
    public async Task RevokeAsync(
        RefreshToken token,
        string? ipAddress,
        string? replacementTokenHash,
        CancellationToken cancellationToken = default)
    {
        token.RevokedAtUtc = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReplacedByTokenHash = replacementTokenHash;

        await _context.SaveChangesAsync(cancellationToken);
    }

    //cleanup unused/old tokens
    public async Task CleanupAsync(CancellationToken cancellationToken = default)
    {
        //var cutoff = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenCutOffDays);

        //var tokens = _context.RefreshTokens
        //    .Where(t => !t.IsActive && t.RevokedAtUtc < cutoff || t.ExpiresAtUtc < DateTime.UtcNow);

        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(_tokenSettings.RefreshTokenCutOffDays);

        var tokens = _context.RefreshTokens
            .Where(t =>
                (t.RevokedAtUtc != null && t.RevokedAtUtc < cutoff) ||
                t.ExpiresAtUtc < now);

        _context.RefreshTokens.RemoveRange(tokens);

        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task LogSecurityEvent(
        string? userId, 
        string eventType, 
        string? ipAddress, 
        string details, 
        CancellationToken cancellationToken = default)
    {
        var entity = new SecurityEvent
        {
            UserId = userId,
            EventType = eventType,
            IpAddress = ipAddress,
            Details = details,
            Timestamp = DateTimeOffset.UtcNow
        };

        _context.SecurityEvents.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

}
