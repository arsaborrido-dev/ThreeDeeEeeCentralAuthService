using AuthApi.Application.Helper;

namespace AuthApi.Application.Portfolio.Projects.DTOs;
public sealed class ProjectResponse
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string LongDescription { get; init; } = string.Empty;
    public string ProjectDemonstration { get; set; } = string.Empty;
    public string? ThumbnailImageUrl { get; init; }

    [HttpsUrl]
    public string? GithubUrl { get; init; }

    [HttpsUrl]
    public string? LiveDemoUrl { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public bool IsFeatured { get; init; }
    public List<ProjectScreenshotSummary> Screenshots { get; init; } = [];
    public List<ProjectSkillSummary> Skills { get; init; } = [];
    public List<ProjectTechnologySummary> Technologies { get; init; } = [];
}    
