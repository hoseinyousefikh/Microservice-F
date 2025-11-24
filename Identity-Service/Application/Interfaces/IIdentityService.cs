using System.Threading.Tasks;
using System.Collections.Generic;
using Identity_Service.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity_Service.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<IList<Domain.Entities.User>> GetAllUsersAsync();
        // User operations
        Task<Domain.Entities.User> FindByIdAsync(Guid userId);
        Task<Domain.Entities.User> FindByNameAsync(string userName);
        Task<Domain.Entities.User> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(Domain.Entities.User user, string password);
        Task<bool> IsEmailConfirmedAsync(Domain.Entities.User user);
        Task<IList<string>> GetRolesAsync(Domain.Entities.User user);
        Task<IdentityResult> CreateAsync(Domain.Entities.User user, string password);
        Task<IdentityResult> UpdateAsync(Domain.Entities.User user);
        Task<IdentityResult> DeleteAsync(Domain.Entities.User user);
        Task<IdentityResult> AddToRoleAsync(Domain.Entities.User user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(Domain.Entities.User user, string role);
        Task<string> GenerateEmailConfirmationTokenAsync(Domain.Entities.User user);
        Task<string> GeneratePasswordResetTokenAsync(Domain.Entities.User user);
        Task<IdentityResult> ResetPasswordAsync(Domain.Entities.User user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(Domain.Entities.User user, string currentPassword, string newPassword);
        Task<DateTimeOffset?> GetLockoutEndDateAsync(Domain.Entities.User user);
        Task<IdentityResult> AccessFailedAsync(Domain.Entities.User user);
        Task ResetAccessFailedCountAsync(Domain.Entities.User user);
        Task<bool> IsLockedOutAsync(Domain.Entities.User user);
        Task<DateTimeOffset?> GetLastLoginDateAsync(Domain.Entities.User user);
        Task SetLastLoginDateAsync(Domain.Entities.User user);

        // Role operations
        Task<Domain.Entities.Role> FindRoleByIdAsync(Guid roleId);
        Task<Domain.Entities.Role> FindRoleByNameAsync(string roleName);
        Task<IList<Domain.Entities.Role>> GetAllRolesAsync();
        Task<IdentityResult> CreateRoleAsync(Domain.Entities.Role role);
        Task<IdentityResult> UpdateRoleAsync(Domain.Entities.Role role);
        Task<IdentityResult> DeleteRoleAsync(Domain.Entities.Role role);
    }
}