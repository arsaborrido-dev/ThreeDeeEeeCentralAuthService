namespace AuthApi.Domain.Entitities;

public class SecurityEvent
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string EventType { get; set; } = string.Empty; // e.g., "RevokedTokenUsed"
    public string? IpAddress { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}