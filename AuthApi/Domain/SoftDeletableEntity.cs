namespace AuthApi.Domain;
public abstract class SoftDeletableEntity : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
}
