namespace AuthApi.Application.Common.Models;

public sealed class RefreshTokenSettings
{
    public int RefreshTokenExpireDays { get; set; } = 7;
    public int RefreshTokenCutOffDays { get; set; } = -30;
    public string CookiePath { get; set; } = "/api/auth";
}
