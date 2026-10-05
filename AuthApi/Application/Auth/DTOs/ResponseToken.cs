namespace AuthApi.Application.Auth.DTOs;
public sealed record ResponseToken(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
