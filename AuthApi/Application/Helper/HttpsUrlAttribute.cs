using AuthApi.Infrastracture.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Application.Helper;

public class HttpsUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (UrlValidator.IsValidHttpsUrl(value as string))
            return ValidationResult.Success;

        return new ValidationResult("The URL must be a valid HTTPS URL.");
    }
}