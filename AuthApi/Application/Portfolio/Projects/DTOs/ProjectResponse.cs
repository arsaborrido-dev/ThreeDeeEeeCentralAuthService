namespace AuthApi.Application.Portfolio.Projects.DTOs
{

    public sealed class ProjectResponse
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string ShortDescription { get; init; } = string.Empty;
        public string LongDescription { get; init; } = string.Empty;
        public string? ThumbnailImageUrl { get; init; }
        public string? GithubUrl { get; init; }
        public string? LiveDemoUrl { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
        public bool IsFeatured { get; init; }
        public List<string> ScreenshotUrls { get; init; } = [];
        public List<string> Skills { get; init; } = [];
        public List<string> Technologies { get; init; } = [];
    }    
}
