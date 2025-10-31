using System.Security.Claims;

namespace IdentityService.BLL.Abstractions.TokenProvider;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}