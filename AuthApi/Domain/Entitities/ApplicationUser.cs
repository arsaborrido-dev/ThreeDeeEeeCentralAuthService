using Microsoft.AspNetCore.Identity;

namespace AuthApi.Domain.Entitities;
public class ApplicationUser : IdentityUser 
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();
}