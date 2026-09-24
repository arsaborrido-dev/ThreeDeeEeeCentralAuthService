namespace AuthApi.Application.Portfolio.Skills.DTOs
{
    public sealed class SkillProjectSummary
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string ShortDescription { get; init; } = string.Empty;
        public string? ThumbnailImageUrl { get; init; }
    }
}
