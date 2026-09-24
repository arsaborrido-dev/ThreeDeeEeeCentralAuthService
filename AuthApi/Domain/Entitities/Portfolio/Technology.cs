namespace AuthApi.Domain.Entitities.Portfolio;
public class Technology : SoftDeletableEntity
{
    public long TechnologyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Project> Projects { get; set; }
        = new List<Project>();
}
