using System.ComponentModel.DataAnnotations;

namespace AuthApi.Application.Portfolio.Contacts.DTOs;
public sealed class ContactMessageRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(5000, MinimumLength = 1)]
    public string Message { get; init; } = string.Empty;

    [Required]
    [StringLength(2048)]
    public string CaptchaToken { get; init; } = string.Empty;
}
