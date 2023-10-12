using System.Security.Claims;

namespace Challenge.Domain.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(string user, string pass, double minutes = 60);
        string GenerateTokenIncludeUser(string currentUserSession, string user, string pass, double minutes = 60);
        ClaimsPrincipal GetTokenInfo(string token);
    }
}
