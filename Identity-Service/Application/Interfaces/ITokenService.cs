using System.Security.Claims;
using System.Threading.Tasks;
using Identity_Service.Domain.Entities;

namespace Identity_Service.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
    }
}