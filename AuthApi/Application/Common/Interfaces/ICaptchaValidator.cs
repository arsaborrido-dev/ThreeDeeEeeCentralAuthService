namespace AuthApi.Application.Common.Interfaces;
public interface ICaptchaValidator
{
    Task<bool> VerifyAsync(string token);
}
