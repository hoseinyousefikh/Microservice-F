using FluentValidation;
using FluentValidation.Results;
using IdentityService._01_Domain.Core.Contracts;
using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using IdentityService._03_EndPoints.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService._03_EndPoints.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IValidator<CreateUserRequestDto> _createUserValidator;
        private readonly IValidator<UpdateUserRequestDto> _updateUserValidator;

        public AdminController(
            IAdminService adminService,
            IValidator<CreateUserRequestDto> createUserValidator,
            IValidator<UpdateUserRequestDto> updateUserValidator)
        {
            _adminService = adminService;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams pagination)
        {
            var result = await _adminService.GetAllUsersAsync(pagination);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            ValidationResult validationResult = await _createUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList() });

            var result = await _adminService.CreateUserAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequestDto request)
        {
            ValidationResult validationResult = await _updateUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList() });

            var result = await _adminService.UpdateUserAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var result = await _adminService.ActivateUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var result = await _adminService.DeactivateUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("users/{id}/confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail(string id)
        {
            var result = await _adminService.ConfirmUserEmailAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}