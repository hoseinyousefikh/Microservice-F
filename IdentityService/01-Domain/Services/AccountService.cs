using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // اضافه شد
using System.Linq; // اضافه شد
using System.Threading.Tasks; // اضافه شد

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

        public async Task<ApiResponseDto<ChangeEmailResponseDto>> ChangeEmailAsync(string userId, ChangeEmailRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<ChangeEmailResponseDto> { Success = false, Message = "User not found" };
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

                var responseDto = new ChangeEmailResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    NewEmail = request.NewEmail
                };

                return new ApiResponseDto<ChangeEmailResponseDto>
                {
                    Success = true,
                    Message = "Email change request sent. Please check your new email for confirmation.",
                    Data = responseDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing email");
                return new ApiResponseDto<ChangeEmailResponseDto> { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                // پیام عمومی برای جلوگیری از حملات شمارش
                if (user == null || !user.EmailConfirmed)
                {
                    return new ApiResponseDto { Success = true, Message = "If your email is registered, you'll receive a code." };
                }

                // توکن ریست پسورد را تولید می‌کنیم
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // توکن را در دیتابیس به همراه زمان انقضا ذخیره می‌کنیم
                user.ResetPasswordToken = token;
                user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1); // کد به مدت 1 ساعت معتبر است
                await _userManager.UpdateAsync(user);

                // ارسال کد به ایمیل کاربر
                await _emailService.SendPasswordResetEmailAsync(user.Email, token);

                return new ApiResponseDto { Success = true, Message = "Password reset code has been sent to your email." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                return new ApiResponseDto { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordWithCodeRequestDto request)
        {
            try
            {
                // کاربر را بر اساس توکن و زمان انقضا پیدا می‌کنیم
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.ResetPasswordToken == request.Token && u.ResetPasswordTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "Invalid or expired code." };
                }

                // رمز عبور را با استفاده از توکن ریست می‌کنیم
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Password reset failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // توکن و زمان انقضا را پاک می‌کنیم تا دیگر قابل استفاده نباشد
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                // تمام توکن‌های فعال کاربر را باطل می‌کنیم
                await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(user.Id, "Password reset");
                await _unitOfWork.CommitAsync();

                return new ApiResponseDto { Success = true, Message = "Password reset successfully." };
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