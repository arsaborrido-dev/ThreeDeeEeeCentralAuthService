namespace AuthApi.Application.Portfolio.Projects.DTOs;
public sealed class ProjectSkillSummary
{
    public long Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string SkillCategory { get; set; } = string.Empty;
}
