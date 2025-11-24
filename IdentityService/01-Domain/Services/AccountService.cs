using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService._01_Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Password change failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId, "Password changed");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "Password changed successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ChangeEmailAsync(string userId, ChangeEmailRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
                // In a real app, send this token via email
                _logger.LogInformation($"Email change token for {user.Email}: {token}");

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Email change request sent. Please check your new email for confirmation."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing email");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    return new ApiResponseDto { Success = true, Message = "If your email is registered, you'll receive a reset link" };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // In a real app, send this token via email
                _logger.LogInformation($"Password reset token for {user.Email}: {token}");

                return new ApiResponseDto { Success = true, Message = "Password reset link sent if email exists" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                    u.ResetPasswordToken == token && u.ResetPasswordTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "Invalid or expired token" };
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Password reset failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(user.Id, "Password reset");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "Password reset successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> EditProfileAsync(string userId, EditProfileRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "User not found" };
                }

                user.FirstName = request.FirstName ?? user.FirstName;
                user.LastName = request.LastName ?? user.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Profile update failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new ApiResponseDto { Success = true, Message = "Profile updated successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing profile");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> DeleteAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
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
                        Message = "Account deletion failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId, "Account deleted");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "Account deleted successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }
    }
}
