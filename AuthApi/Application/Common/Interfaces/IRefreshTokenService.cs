using AuthApi.Domain.Entitities;

namespace AuthApi.Application.Common.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<(string RawToken, RefreshToken Entity)> CreateAsync(
        string userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetActiveAsync(
            string rawToken,
            CancellationToken cancellationToken = default);
        Task RevokeAsync(
            RefreshToken token,
            string? ipAddress,
            string? replacementTokenHash,
            CancellationToken cancellationToken = default);
    }
}
