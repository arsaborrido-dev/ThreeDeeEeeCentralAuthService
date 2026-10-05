using AuthApi.Application.Auth.DTOs;
using AuthApi.Application.Common.Models;

namespace AuthApi.Application.Auth;

public interface IAuthService
{
    Task<ResponseResult<ResponseToken>> LoginAsync(LoginDto dto, string ipAddress, CancellationToken cancellationToken = default);            
    //Task<ResponseResult<ResponseToken>> RefreshAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    Task<ResponseResult> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    //Task RevokeAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
        Task<ResponseResult> RevokeTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);
    Task<ResponseResult> CleanupTokensAsync(
        CancellationToken cancellationToken = default);
    Task<ResponseResult<ResponseToken>> RefreshTokenAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<ResponseResult> LogoutAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default);

}