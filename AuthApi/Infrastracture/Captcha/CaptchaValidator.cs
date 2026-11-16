using AuthApi.Application.Common.Interfaces;
using System.Text.Json;

namespace AuthApi.Infrastracture.Captcha;
public class CaptchaValidator : ICaptchaValidator
{
    private readonly HttpClient _http;
    private readonly string _secretKey;
    public CaptchaValidator(
        IConfiguration config,
        HttpClient http)
    {
        _http = http;
        _secretKey = config["Captcha:SecretKey"] ?? throw new InvalidOperationException("Captcha:SecretKey is not configured.");
    }
    public async Task<bool> VerifyAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            var response = await _http.PostAsync(
                "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["secret"] = _secretKey,
                        ["response"] = token
                    }));

            if (!response.IsSuccessStatusCode)
                return false;

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var root = document.RootElement;

            if (!root.TryGetProperty("success", out var success) ||
                !success.GetBoolean())
                return false;

            if (!root.TryGetProperty("hostname", out var hostnameElement))
                return false;

            var hostname = hostnameElement.GetString();

            var allowedHostnames = new[]
            {
                "chaosaborrido.up.railway.app",
                "YOUR-STAGING-MVC-HOSTNAME"
            };

            return hostname != null &&
                   allowedHostnames.Contains(
                       hostname,
                       StringComparer.OrdinalIgnoreCase);
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }


        //var response = await _http.PostAsync(
        //    "https://challenges.cloudflare.com/turnstile/v0/siteverify",
        //    new FormUrlEncodedContent(
        //        new Dictionary<string, string>
        //        {
        //            ["secret"] = _secretKey,
        //            ["response"] = token
        //        }));

        //var json = await response.Content.ReadAsStringAsync();

        //if (!response.IsSuccessStatusCode)
        //    return false;

        //using var document = JsonDocument.Parse(json);

        //var success = document.RootElement
        //    .GetProperty("success")
        //    .GetBoolean();

        //var hostname = document.RootElement
        //    .GetProperty("hostname")
        //    .GetString();

        //return success;
    }
}
