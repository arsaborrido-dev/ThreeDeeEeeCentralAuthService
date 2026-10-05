

namespace AuthApi.Application.Portfolio.Technologies.DTOs;
public sealed class TechnologyResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<TechnologyProjectSummary> Projects { get; init; } = [];
}
