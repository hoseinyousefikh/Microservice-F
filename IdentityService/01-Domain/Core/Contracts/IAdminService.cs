using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;
using IdentityService._03_EndPoints.DTOs.Shared;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IAdminService
    {
        Task<ApiResponseDto<IEnumerable<UserResponseDto>>> GetAllUsersAsync(PaginationParams pagination);
        Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(string userId);
        Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request);
        Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(string userId, UpdateUserRequestDto request);
        Task<ApiResponseDto> DeleteUserAsync(string userId);
        Task<ApiResponseDto> ActivateUserAsync(string userId);
        Task<ApiResponseDto> DeactivateUserAsync(string userId);
        Task<ApiResponseDto> ConfirmUserEmailAsync(string userId); // متد جدید
    }
}
