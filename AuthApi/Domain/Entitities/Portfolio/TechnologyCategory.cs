namespace AuthApi.Domain.Entitities.Portfolio;
public class TechnologyCategory : SoftDeletableEntity
{
    public long TechnologyCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Technology> Technologies { get; set; }
        = new List<Technology>();
}
