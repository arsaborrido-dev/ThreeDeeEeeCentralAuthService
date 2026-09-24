using System.ComponentModel.DataAnnotations;

namespace AuthApi.Application.Auth.DTOs;

//public record RegisterDto(string Email, string Password, string FirstName, string LastName);
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}