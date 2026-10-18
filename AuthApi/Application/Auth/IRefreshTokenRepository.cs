using AuthApi.Domain.Entitities;

namespace AuthApi.Application.Auth;
public interface IRefreshTokenRepository
{
    Task<(string RawToken, RefreshToken Entity)> CreateAsync(
        //string userId,
        ApplicationUser user,
        string? ipAddress,
        bool isRefresh = false,
        RefreshToken? token = null,
        CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetTokenAsync(
        string rawToken,
        CancellationToken cancellationToken = default);
    Task RevokeAsync(
        RefreshToken token,
        string? ipAddress,
        string? replacementTokenHash,
        CancellationToken cancellationToken = default);
    Task CleanupAsync(
        CancellationToken cancellationToken = default);
    Task LogSecurityEvent(
        string? userId,
        string eventType,
        string? ipAddress,
        string details,
        CancellationToken cancellationToken = default);
}
