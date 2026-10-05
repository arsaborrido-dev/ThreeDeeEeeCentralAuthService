namespace AuthApi.Application.Portfolio.Skills.DTOs;
public sealed class SkillResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SkillCategory { get; init; } = string.Empty;
    public List<SkillProjectSummary> Projects { get; init; } = [];
}
