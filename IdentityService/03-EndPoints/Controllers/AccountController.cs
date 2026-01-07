using FluentValidation;
using FluentValidation.Results;
using IdentityService._01_Domain.Core.Contracts;
using IdentityService._01_Domain.Core.Enums;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService._03_EndPoints.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService; // Added for email confirmation
        private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
        private readonly IValidator<ChangeEmailRequestDto> _changeEmailValidator;
        private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
        private readonly IValidator<EditProfileRequestDto> _editProfileValidator;

        public AccountController(
            IAccountService accountService,
            IAuthService authService, // Added for email confirmation
            IValidator<ChangePasswordRequestDto> changePasswordValidator,
            IValidator<ChangeEmailRequestDto> changeEmailValidator,
            IValidator<ForgotPasswordRequestDto> forgotPasswordValidator,
            IValidator<EditProfileRequestDto> editProfileValidator)
        {
            _accountService = accountService;
            _authService = authService; // Added for email confirmation
            _changePasswordValidator = changePasswordValidator;
            _changeEmailValidator = changeEmailValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _editProfileValidator = editProfileValidator;
        }
        // در پروژه IdentityService، در AccountController

        [HttpGet("confirm-change-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail([FromQuery] string userId, [FromQuery] string token, [FromQuery] string newEmail)
        {
            var result = await _authService.ConfirmChangeEmailAsync(userId, token, newEmail);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            ValidationResult validationResult = await _changePasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList() });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountService.ChangePasswordAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // در پروژه IdentityService، در AccountController

        [HttpPost("change-email")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto request)
        {
            // userId را از توکن کاربر استخراج می‌کنیم
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var result = await _accountService.ChangeEmailAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            ValidationResult validationResult = await _forgotPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList() });

            var result = await _accountService.ForgotPasswordAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithCodeRequestDto request)
        {
            // این متد اکنون یک آبجکت با توکن و رمز عبور جدید دریافت می‌کند
            var result = await _accountService.ResetPasswordAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileRequestDto request)
        {
            ValidationResult validationResult = await _editProfileValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList() });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountService.EditProfileAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountService.DeleteAccountAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}