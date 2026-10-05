using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Contacts.DTOs;
using AuthApi.Domain.Entitities.Portfolio;
using AuthApi.Application.Helper;

namespace AuthApi.Application.Portfolio.Contacts;
public sealed class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly ILogger<ContactService> _logger;
    public ContactService(
        IContactRepository contactRepository,
        ICaptchaValidator captchaValidator,
        ILogger<ContactService> logger)
    {
        _contactRepository = contactRepository;
        _captchaValidator = captchaValidator;
        _logger = logger;
    }
    public async Task<ResponseResult> SaveContactMessageAsync(
        ContactMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        //validate request input
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100)
        {
            _logger.LogWarning("[ContactService:SaveContactMessageAsync] - Invalid name.");

            return ResponseResult.Failure(
                "INVALID_NAME",
                "Invalid name.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 320)
        {
            _logger.LogWarning("[ContactService:SaveContactMessageAsync] - Invalid email.");

            return ResponseResult.Failure(
                "INVALID_EMAIL",
                "Invalid email address.");
        }

        if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 5000)
        {
            _logger.LogWarning("[ContactService:SaveContactMessageAsync] - Invalid message.");

            return ResponseResult.Failure(
                "INVALID_MESSAGE",
                "Invalid message.");
        }

        if (string.IsNullOrWhiteSpace(request.CaptchaToken) || request.CaptchaToken.Length > 2048)
        {
            _logger.LogWarning("[ContactService:SaveContactMessageAsync] - Invalid CAPTCHA length.");

            return ResponseResult.Failure(
                "INVALID_CAPTCHA",
                "Invalid captcha.");
        }

        //verify captcha
        var captchaValid = await _captchaValidator.VerifyAsync(
            request.CaptchaToken);

        if (!captchaValid)
        {
            _logger.LogWarning("[ContactService:SaveContactMessageAsync] - Invalid CAPTCHA.");

            return ResponseResult.Failure(
               "INVALID_CAPTCHA",
               "Invalid captcha.");
        }

        var message = new ContactMessage
        {
            Name = HtmlSanitizerHelper.Sanitize(request.Name.Trim()),
            Email = HtmlSanitizerHelper.Sanitize(request.Email.Trim()),
            Message = HtmlSanitizerHelper.Sanitize(request.Message.Trim()),
            CreatedAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        await _contactRepository.SaveContactMessageAsync(
                message,
                cancellationToken);            

        return ResponseResult.Success();
    }
}
