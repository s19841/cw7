using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cw7.Services
{
    public interface IAuthenticationService
    {
        JwtSecurityToken Login(string username, string password);
        bool UpdateRefreshToken(string username, string refreshToken);
        ClaimsPrincipal ValidateJwtAndGetClaimsPrincipal(string jwt);
        JwtSecurityToken RefreshJwt(string username, string refreshToken);
    }
}