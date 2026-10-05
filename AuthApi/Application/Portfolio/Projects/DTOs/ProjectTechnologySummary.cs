namespace AuthApi.Application.Portfolio.Projects.DTOs;
public class ProjectTechnologySummary
{
    public long Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string TechnologyCategory { get; set; } = string.Empty;
}
