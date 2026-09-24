namespace AuthApi.Domain.Entitities;
public sealed class RefreshToken
{
    public long Id { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public bool IsActive =>
        RevokedAtUtc == null &&
        DateTimeOffset.UtcNow < ExpiresAtUtc;
}
