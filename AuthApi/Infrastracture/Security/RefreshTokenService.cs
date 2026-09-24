using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Domain.Entitities;
using AuthApi.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthApi.Infrastracture.Security
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _settings;
        public RefreshTokenService(
            ApplicationDbContext context, 
            IOptions<JwtSettings> options)
        {
            _context = context;
            _settings = options.Value;
        }
        public async Task<(string RawToken, RefreshToken Entity)> CreateAsync(
            string userId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var rawToken = GenerateToken();

            var entity = new RefreshToken
            {
                //Id = Guid.NewGuid(),
                TokenHash = HashToken(rawToken),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(
                    _settings.RefreshTokenExpireDays),
                CreatedByIp = ipAddress
            };

            _context.RefreshTokens.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return (rawToken, entity);
        }
        public async Task<RefreshToken?> GetActiveAsync(
            string rawToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rawToken))
                return null;

            var hash = HashToken(rawToken);

            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(
                    x => x.TokenHash == hash,
                    cancellationToken);

            if (token == null)
                return null;

            if (!token.IsActive)
                return null;

            return token;
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
}
