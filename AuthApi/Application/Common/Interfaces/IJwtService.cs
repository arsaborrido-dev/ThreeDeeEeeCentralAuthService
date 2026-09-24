using System.Security.Claims;

namespace AuthApi.Application.Common.Interfaces
{

    public interface IJwtService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        ClaimsPrincipal? ValidateAccessToken(string token);
    }    
}
