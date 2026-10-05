namespace AuthApi.Application.Auth.DTOs;
public sealed class CurrentUserDto
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; }

    public CurrentUserDto(string? id, string? email, IReadOnlyCollection<string> roles)
    {
        Id = id;
        Email = email;
        Roles = roles;
    }
}
