namespace AuthApi.Domain.Entitities.Portfolio;
public class ProjectScreenshot : SoftDeletableEntity
{
    public long ProjectScreenshotId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
