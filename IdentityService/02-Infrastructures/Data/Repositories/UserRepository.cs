using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace IdentityService._02_Infrastructures.Data.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken &&
                                          u.RefreshTokenExpiry > DateTime.UtcNow);
        }

        public async Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.Status == AccountStatus.Active && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateLastLoginAsync(string userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                Update(user);
            }
        }
    }
}
