namespace AuthApi.Application.Portfolio.Address.DTOs;

public sealed class AddressResponseDto
{
    public long Id { get; init; }
    public string ZipCode { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
}