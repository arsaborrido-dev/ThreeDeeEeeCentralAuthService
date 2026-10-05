namespace AuthApi.Application.Portfolio.Technologies.DTOs;
public sealed class TechnologyProjectSummary
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string? ThumbnailImageUrl { get; init; }
}
