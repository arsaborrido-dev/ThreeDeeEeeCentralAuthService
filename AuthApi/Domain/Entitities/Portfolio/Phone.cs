namespace AuthApi.Domain.Entitities.Portfolio;

public class Phone : SoftDeletableEntity, IAuditableEntity
{
    public long PhoneId { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

}
