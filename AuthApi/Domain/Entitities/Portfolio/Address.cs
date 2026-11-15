
namespace AuthApi.Domain.Entitities.Portfolio;

public class Address : SoftDeletableEntity, IAuditableEntity
{
    public long AddressId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;    
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

}
