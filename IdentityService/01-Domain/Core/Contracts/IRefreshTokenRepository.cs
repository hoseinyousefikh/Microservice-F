using IdentityService._01_Domain.Core.Entities;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RevokeAllUserTokensAsync(string userId, string reason);
        Task CleanupExpiredTokensAsync();
    }
}
