using IdentityService._01_Domain.Core.Entities;
using System.Security.Claims;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IJwtService
    {
        string GenerateAccessToken(ApplicationUser user, List<string> roles);
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime GetRefreshTokenExpiryTime();
    }
}
