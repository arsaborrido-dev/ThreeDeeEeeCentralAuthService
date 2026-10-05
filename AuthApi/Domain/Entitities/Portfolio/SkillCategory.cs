namespace AuthApi.Domain.Entitities.Portfolio;
public class SkillCategory : SoftDeletableEntity
{
    public long SkillCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Skill> Skills { get; set; }
        = new List<Skill>();
}
