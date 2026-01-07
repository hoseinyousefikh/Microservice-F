using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponseDto> LogoutAsync(string userId);
        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<ApiResponseDto> ConfirmChangeEmailAsync(string userId, string token, string newEmail);
    }
}
