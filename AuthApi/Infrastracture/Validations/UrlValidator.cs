namespace AuthApi.Infrastracture.Validations;

public static class UrlValidator
{
    public static bool IsValidHttpsUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!Uri.TryCreate(
                value,
                UriKind.Absolute,
                out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttps;
    }
}