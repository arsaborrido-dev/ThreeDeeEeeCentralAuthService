namespace AuthApi.Application.Portfolio.Projects.DTOs;
public sealed class ProjectScreenshotSummary
{
    public long Id { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
}
