namespace AuthApi.Domain.Entitities.Portfolio;
public class Technology : SoftDeletableEntity
{
    public long TechnologyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long TechnologyCategoryId { get; set; }
    public TechnologyCategory TechnologyCategory { get; set; } = null!;
    public ICollection<Project> Projects { get; set; }
        = new List<Project>();
}
