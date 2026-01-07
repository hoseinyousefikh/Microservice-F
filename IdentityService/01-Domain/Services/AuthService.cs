using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Entities;
using IdentityService._01_Domain.Core.Enums;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using Microsoft.AspNetCore.Identity;

namespace IdentityService._01_Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Email already exists",
                        Errors = new List<string> { "Email already registered" }
                    };
                }

                // تعیین AccountStatus بر اساس نقش
                AccountStatus status;
                switch (request.Role)
                {
                    case UserRole.Customer:
                    case UserRole.Seller:
                        status = AccountStatus.Active;
                        break;
                    case UserRole.Moderator:
                        status = AccountStatus.PendingActivation;
                        break;
                    case UserRole.Admin:
                        status = AccountStatus.Suspended;
                        break;
                    default:
                        status = AccountStatus.PendingActivation; // پیش‌فرض
                        break;
                }

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailConfirmed = false,
                    Status = status
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // افزودن نقش کاربر
                await _userManager.AddToRoleAsync(user, request.Role.ToString());

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _logger.LogInformation($"Email confirmation token for {user.Email}: {token}");

                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = true,
                    Message = "Registration successful. Please check your email for confirmation.",
                    Data = new AuthResponseDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        RequiresEmailConfirmation = true
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || user.IsDeleted)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    await _userManager.AccessFailedAsync(user);
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                // فقط برای فروشنده‌ها نیاز به تأیید ایمیل است
                if (userRoles.Contains(UserRole.Seller.ToString()) && !user.EmailConfirmed)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Seller must confirm email before logging in."
                    };
                }

                // مشتریان می‌توانند بدون تأیید ایمیل وارد شوند
                if (userRoles.Contains(UserRole.Customer.ToString()) && !user.EmailConfirmed)
                {
                    _logger.LogWarning($"Customer {user.Email} logging in without email confirmation");
                }

                if (user.Status != AccountStatus.Active)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Account is not active"
                    };
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Account locked out"
                    };
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                await _unitOfWork.UserRepository.UpdateLastLoginAsync(user.Id);

                var accessToken = _jwtService.GenerateAccessToken(user, userRoles.ToList());
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiryTime();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = refreshTokenExpiry;
                await _userManager.UpdateAsync(user);

                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Expiration = refreshTokenExpiry,
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = userRoles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }
        public async Task<ApiResponseDto> LogoutAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = DateTime.MinValue;
                    await _userManager.UpdateAsync(user);
                }

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Logout successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "An error occurred during logout"
                };
            }
        }
        //public async Task<ApiResponseDto> ConfirmChangeEmailAsync(string userId, string token, string newEmail)
        //{
        //    try
        //    {
        //        // --- شروع بخش تست ---
        //        // به طور موقت، توکن دریافتی را نادیده بگیر و از توکن ثابت استفاده کن
        //        token = "CfDJ8BFzOAY04cFHgKh58J2c/wvlkoqppasKJgpDbMwpGq5niLoXAHFktMU8EZkYu09iVPcDNXp2ZaFdc5DbGDUhB0vzC6FzHKLs4MwqWNzEXPOVri/rLZmVDLI3KU/LY/58kyaP7UcAAOus6cMS0NHJEVoup0ZAzZzkvk8szTPz37+aRxF5/MoZud+XN7oo0kQOPeKhNRp/W2/pbNiokJH//bUTWVBVz0ntIBsgXFG4o8n6spQr8vVvEo5TJfb/GTvHbPOgR6mp9/406meX6CNCabc=";
        //        // --- پایان بخش تست ---

        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return new ApiResponseDto { Success = false, Message = "Invalid user ID" };
        //        }

        //        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

        //        if (!result.Succeeded)
        //        {
        //            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //            _logger.LogWarning("EMAIL CHANGE FAILED (HARDCODED TOKEN TEST): User {UserId}. Errors: {Errors}", userId, errors);

        //            return new ApiResponseDto
        //            {
        //                Success = false,
        //                Message = "Email change confirmation failed",
        //                Errors = result.Errors.Select(e => e.Description).ToList()
        //            };
        //        }

        //        user.Status = AccountStatus.Active;
        //        await _userManager.UpdateAsync(user);

        //        return new ApiResponseDto
        //        {
        //            Success = true,
        //            Message = "Email changed and confirmed successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during email change confirmation");
        //        return new ApiResponseDto { Success = false, Message = "An error occurred during email change confirmation" };
        //    }
        //}
        public async Task<ApiResponseDto> ConfirmChangeEmailAsync(string userId, string token, string newEmail)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto { Success = false, Message = "Invalid user ID" };
                }

                // متد ChangeEmailAsync هم ایمیل را تغییر می‌دهد و هم توکن را تأیید می‌کند
                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Email change confirmation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // وضعیت کاربر را در صورت نیاز آپدیت کنید
                user.Status = AccountStatus.Active;
                await _userManager.UpdateAsync(user);

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Email changed and confirmed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email change confirmation");
                return new ApiResponseDto { Success = false, Message = "An error occurred during email change confirmation" };
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);
                if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    };
                }

                // Get user roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Use the two-parameter method
                var newAccessToken = _jwtService.GenerateAccessToken(user, userRoles.ToList());
                var newRefreshToken = _jwtService.GenerateRefreshToken();
                var newRefreshTokenExpiry = _jwtService.GetRefreshTokenExpiryTime();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = newRefreshTokenExpiry;
                await _userManager.UpdateAsync(user);

                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = new AuthResponseDto
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        Expiration = newRefreshTokenExpiry
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = "An error occurred during token refresh"
                };
            }
        }
        public async Task<ApiResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "Email confirmation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                user.Status = AccountStatus.Active;
                await _userManager.UpdateAsync(user);

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "Email confirmed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation");
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "An error occurred during email confirmation"
                };
            }
        }
    }
}