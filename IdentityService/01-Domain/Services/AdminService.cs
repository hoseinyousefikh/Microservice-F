using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using IdentityService._03_EndPoints.DTOs.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService._01_Domain.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<IEnumerable<UserResponseDto>>> GetAllUsersAsync(PaginationParams pagination)
        {
            try
            {
                var query = _userManager.Users
                    .Where(u => !u.IsDeleted)
                    .OrderBy(u => u.CreatedAt);

                var users = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();

                var userDtos = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Status = u.Status.ToString(),
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    Roles = _userManager.GetRolesAsync(u).Result.ToList()
                });

                return new ApiResponseDto<IEnumerable<UserResponseDto>>
                {
                    Success = true,
                    Data = userDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new ApiResponseDto<IEnumerable<UserResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred"
                };
            }
        }
        public async Task<ApiResponseDto> ConfirmUserEmailAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                if (user.EmailConfirmed)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Email is already confirmed"
                    };
                }

                user.EmailConfirmed = true;

                // اگر کاربر در حالت PendingActivation است، آن را به Active تغییر می‌دهیم
                if (user.Status == AccountStatus.PendingActivation)
                {
                    user.Status = AccountStatus.Active;
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Failed to confirm email",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                _logger.LogInformation($"Admin confirmed email for user {user.Email}");

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Email confirmed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "An error occurred while confirming email"
                };
            }
        }
        public async Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto<UserResponseDto>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                };

                return new ApiResponseDto<UserResponseDto>
                {
                    Success = true,
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID");
                return new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = "An error occurred"
                };
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailConfirmed = true,
                    Status = AccountStatus.Active
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto<UserResponseDto>
                    {
                        Success = false,
                        Message = "User creation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                if (request.Roles?.Any() == true)
                {
                    await _userManager.AddToRolesAsync(user, request.Roles);
                }

                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                };

                return new ApiResponseDto<UserResponseDto>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = "An error occurred"
                };
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(string userId, UpdateUserRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto<UserResponseDto>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                user.FirstName = request.FirstName ?? user.FirstName;
                user.LastName = request.LastName ?? user.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto<UserResponseDto>
                    {
                        Success = false,
                        Message = "User update failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                if (request.Roles?.Any() == true)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, request.Roles);
                }

                var userDto = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                };

                return new ApiResponseDto<UserResponseDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return new ApiResponseDto<UserResponseDto>
                {
                    Success = false,
                    Message = "An error occurred"
                };
            }
        }

        public async Task<ApiResponseDto> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                user.IsDeleted = true;
                user.Status = AccountStatus.Deleted;
                user.RefreshToken = null;
                user.RefreshTokenExpiry = DateTime.MinValue;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "User deletion failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId, "User deleted by admin");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "User deleted successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                user.Status = AccountStatus.Active;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "User activation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new ApiResponseDto { Success = true, Message = "User activated successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                user.Status = AccountStatus.Suspended;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "User deactivation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId, "User deactivated by admin");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "User deactivated successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }
    }
}
