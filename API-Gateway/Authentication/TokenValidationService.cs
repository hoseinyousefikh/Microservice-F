using API_Gateway.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Gateway.Authentication
{
    public interface ITokenValidationService
    {
        Task<TokenValidationResult> ValidateTokenAsync(string token);
    }

    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public ClaimsPrincipal User { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TokenValidationService : ITokenValidationService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenValidationService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenValidationResult> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claimsIdentity = new ClaimsIdentity(jwtToken.Claims);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return new TokenValidationResult
                {
                    IsValid = true,
                    User = claimsPrincipal
                };
            }
            catch (SecurityTokenValidationException ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token validation failed"
                };
            }
        }
    }
}
