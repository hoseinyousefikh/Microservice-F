using AutoMapper;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using Identity_Service.Domain.Enums;
using Identity_Service.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfraRole = Identity_Service.Infrastructure.Persistence.Entities.Role;
using InfraUser = Identity_Service.Infrastructure.Persistence.Entities.User;

namespace Identity_Service.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<InfraUser> _userManager;
        private readonly RoleManager<InfraRole> _roleManager;
        private readonly IMapper _mapper;

        public IdentityService(
            UserManager<InfraUser> userManager,
            RoleManager<InfraRole> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // --- User Query Operations ---
        public async Task<IList<Domain.Entities.User>> GetAllUsersAsync()
        {
            var infraUsers = await _userManager.Users.ToListAsync();
            return _mapper.Map<IList<Domain.Entities.User>>(infraUsers);
        }

        public async Task<Domain.Entities.User> FindByIdAsync(Guid userId)
        {
            var infraUser = await _userManager.FindByIdAsync(userId.ToString());
            return _mapper.Map<Domain.Entities.User>(infraUser);
        }

        public async Task<Domain.Entities.User> FindByNameAsync(string userName)
        {
            var infraUser = await _userManager.FindByNameAsync(userName);
            return _mapper.Map<Domain.Entities.User>(infraUser);
        }

        public async Task<Domain.Entities.User> FindByEmailAsync(string email)
        {
            // 1. ابتدا کاربر رو از دیتابیس بگیر
            var infraUser = await _userManager.FindByEmailAsync(email);

            // 2. اگر کاربری وجود نداشت، null برگردون
            if (infraUser == null)
            {
                return null;
            }

            // 3. اگر کاربر وجود داشت، به صورت دستی یک Domain.User بساز
            //    و دیگر از AutoMapper استفاده نکن
            var domainUser = new Domain.Entities.User(
                infraUser.UserName,
                Email.Create(infraUser.Email), // string رو به ValueObject تبدیل کن
                null, // پسورد هش اینجا لازم نیست
                infraUser.FirstName,
                infraUser.LastName,
                infraUser.PhoneNumber != null ? PhoneNumber.Create(infraUser.PhoneNumber) : null
            );

            // 4. حالا ID و سایر پراپرتی‌های کلیدی رو دستی پر کن
            // چون constructor اون‌ها رو نمی‌سازه
            domainUser.GetType().GetProperty("Id")?.SetValue(domainUser, infraUser.Id);
            domainUser.GetType().GetProperty("Status")?.SetValue(domainUser, UserStatus.FromId(infraUser.Status));
            domainUser.GetType().GetProperty("CreatedAt")?.SetValue(domainUser, infraUser.CreatedAt);
            domainUser.GetType().GetProperty("UpdatedAt")?.SetValue(domainUser, infraUser.LastModifiedAt);
            domainUser.GetType().GetProperty("LastLoginAt")?.SetValue(domainUser, infraUser.LastLoginAt);
            domainUser.GetType().GetProperty("EmailConfirmed")?.SetValue(domainUser, infraUser.EmailConfirmed);
            domainUser.GetType().GetProperty("PhoneNumberConfirmed")?.SetValue(domainUser, infraUser.PhoneNumberConfirmed);
            domainUser.GetType().GetProperty("ProfileImageUrl")?.SetValue(domainUser, infraUser.ProfileImageUrl);

            return domainUser;
        }

        // --- User Management & Validation Operations ---
        public async Task<bool> CheckPasswordAsync(Domain.Entities.User user, string password)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null && await _userManager.CheckPasswordAsync(infraUser, password);
        }

        public async Task<bool> IsEmailConfirmedAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null && await _userManager.IsEmailConfirmedAsync(infraUser);
        }

        public async Task<IList<string>> GetRolesAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.GetRolesAsync(infraUser) : new List<string>();
        }

        public async Task<bool> IsInRoleAsync(Domain.Entities.User user, string role)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null && await _userManager.IsInRoleAsync(infraUser, role);
        }

        // --- User CRUD Operations ---
        // در Infrastructure/Services/IdentityService.cs

        public async Task<IdentityResult> CreateAsync(Domain.Entities.User user, string password)
        {
            // به جای استفاده از AutoMapper، آبجکت را به صورت دستی و امن می‌سازیم
            var infraUser = new InfraUser
            {
                UserName = user.Username,
                Email = user.Email?.Value, // ایمیل رو به صورت امن استخراج می‌کنیم
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber?.Value, // شماره تلفن رو به صورت امن استخراج می‌کنیم
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                ProfileImageUrl = user.ProfileImageUrl,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumber != null, // اگر شماره تلفن وجود داشته باشه، تایید شده محسابه میشه
                Status = user.Status.Id // تبدیل enum به int
            };

            var result = await _userManager.CreateAsync(infraUser, password);

            // Update the domain user with the generated ID
            user.GetType().GetProperty("Id")?.SetValue(user, infraUser.Id);

            return result;
        }
        public async Task<IdentityResult> UpdateAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (infraUser == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            _mapper.Map(user, infraUser);
            return await _userManager.UpdateAsync(infraUser);
        }

        public async Task<IdentityResult> DeleteAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.DeleteAsync(infraUser) : IdentityResult.Failed();
        }

        // --- User Role Management ---
        public async Task<IdentityResult> AddToRoleAsync(Domain.Entities.User user, string role)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.AddToRoleAsync(infraUser, role) : IdentityResult.Failed();
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(Domain.Entities.User user, string role)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.RemoveFromRoleAsync(infraUser, role) : IdentityResult.Failed();
        }

        public async Task<IdentityResult> AddToRolesAsync(Domain.Entities.User user, IEnumerable<string> roles)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.AddToRolesAsync(infraUser, roles) : IdentityResult.Failed();
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(Domain.Entities.User user, IEnumerable<string> roles)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.RemoveFromRolesAsync(infraUser, roles) : IdentityResult.Failed();
        }

        // --- User Token & Security Operations ---
        public async Task<string> GenerateEmailConfirmationTokenAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.GenerateEmailConfirmationTokenAsync(infraUser) : string.Empty;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.GeneratePasswordResetTokenAsync(infraUser) : string.Empty;
        }

        public async Task<IdentityResult> ResetPasswordAsync(Domain.Entities.User user, string token, string newPassword)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.ResetPasswordAsync(infraUser, token, newPassword) : IdentityResult.Failed();
        }

        public async Task<IdentityResult> ChangePasswordAsync(Domain.Entities.User user, string currentPassword, string newPassword)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.ChangePasswordAsync(infraUser, currentPassword, newPassword) : IdentityResult.Failed();
        }

        // --- User Lockout Operations ---
        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser?.LockoutEnd;
        }

        public async Task<IdentityResult> AccessFailedAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null ? await _userManager.AccessFailedAsync(infraUser) : IdentityResult.Failed();
        }

        public async Task ResetAccessFailedCountAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (infraUser != null)
            {
                await _userManager.ResetAccessFailedCountAsync(infraUser);
            }
        }

        public async Task<bool> IsLockedOutAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser != null && await _userManager.IsLockedOutAsync(infraUser);
        }

        public async Task SetLockoutEndDateAsync(Domain.Entities.User user, DateTimeOffset? lockoutEnd)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (infraUser != null)
            {
                await _userManager.SetLockoutEndDateAsync(infraUser, lockoutEnd);
            }
        }

        // --- User Session & Tracking Operations ---
        public async Task<DateTimeOffset?> GetLastLoginDateAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return infraUser?.LastLoginAt;
        }

        public async Task SetLastLoginDateAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (infraUser != null)
            {
                infraUser.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(infraUser);
            }
        }

        public async Task UpdateSecurityStampAsync(Domain.Entities.User user)
        {
            var infraUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (infraUser != null)
            {
                await _userManager.UpdateSecurityStampAsync(infraUser);
            }
        }

        // --- Role Query Operations ---
        public async Task<Domain.Entities.Role> FindRoleByIdAsync(Guid roleId)
        {
            var infraRole = await _roleManager.FindByIdAsync(roleId.ToString());
            return _mapper.Map<Domain.Entities.Role>(infraRole);
        }

        public async Task<Domain.Entities.Role> FindRoleByNameAsync(string roleName)
        {
            var infraRole = await _roleManager.FindByNameAsync(roleName);
            return _mapper.Map<Domain.Entities.Role>(infraRole);
        }

        public async Task<IList<Domain.Entities.Role>> GetAllRolesAsync()
        {
            var infraRoles = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<IList<Domain.Entities.Role>>(infraRoles);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        // --- Role CRUD Operations ---
        public async Task<IdentityResult> CreateRoleAsync(Domain.Entities.Role role)
        {
            var infraRole = _mapper.Map<InfraRole>(role);
            var result = await _roleManager.CreateAsync(infraRole);

            // Update the domain role with the generated ID
            role.GetType().GetProperty("Id")?.SetValue(role, infraRole.Id);

            return result;
        }

        public async Task<IdentityResult> UpdateRoleAsync(Domain.Entities.Role role)
        {
            var infraRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (infraRole == null) return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

            _mapper.Map(role, infraRole);
            return await _roleManager.UpdateAsync(infraRole);
        }

        public async Task<IdentityResult> DeleteRoleAsync(Domain.Entities.Role role)
        {
            var infraRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            return infraRole != null ? await _roleManager.DeleteAsync(infraRole) : IdentityResult.Failed();
        }
    }
}