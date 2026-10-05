namespace AuthApi.Domain.Entitities.Portfolio;
public class Skill : SoftDeletableEntity
{
    public long SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long SkillCategoryId { get; set; }
    public SkillCategory SkillCategory { get; set; } = null!;
    public ICollection<Project> Projects { get; set; }
        = new List<Project>();
}