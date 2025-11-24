using IdentityService._01_Domain.Core.Entities;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken);
        Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
        Task UpdateLastLoginAsync(string userId);
    }
}
