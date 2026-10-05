
namespace AuthApi.Domain.Entitities.Portfolio;
public class Project : SoftDeletableEntity
{
    public long ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string ProjectDemonstration { get; set; } = string.Empty;
    public string? ThumbnailImageUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public bool IsFeatured { get; set; }
    public ICollection<ProjectScreenshot> Screenshots { get; set; }
        = new List<ProjectScreenshot>();
    public ICollection<Skill> Skills { get; set; }
        = new List<Skill>();
    public ICollection<Technology> Technologies { get; set; }
        = new List<Technology>();

    //public long ProjectId { get; set; }
    //public string Title { get; set; } = string.Empty;
    //public string ShortDescription { get; set; } = string.Empty;
    //public string LongDescription { get; set; } = string.Empty;
    //public string? ThumbnailImageUrl { get; set; }
    //public string? GitHubUrl { get; set; }
    //public string? LiveDemoUrl { get; set; }
    //public DateTime CreatedDate { get; set; }
    //public bool IsFeatured { get; set; }
    //public ICollection<ProjectScreenshot> Screenshots { get; set; }
    //    = new List<ProjectScreenshot>();
    //public ICollection<Skill> Skills { get; set; }
    //    = new List<Skill>();
    //public ICollection<Technology> Technologies { get; set; }
    //    = new List<Technology>();
}    
