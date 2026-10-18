using System.ComponentModel.DataAnnotations;

namespace AuthApi.Application.Auth.DTOs;

public record LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}