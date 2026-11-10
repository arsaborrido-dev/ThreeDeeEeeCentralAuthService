namespace AuthApi.Application.Portfolio.Phone.DTOs;

public sealed class PhoneResponseRequestDto
{
    public long Id { get; init; }
    public string AreaCode { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
}
