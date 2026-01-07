using IdentityService._03_EndPoints.DTOs.Requests;
using IdentityService._03_EndPoints.DTOs.Responses;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IAccountService
    {
        Task<ApiResponseDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
        Task<ApiResponseDto<ChangeEmailResponseDto>> ChangeEmailAsync(string userId, ChangeEmailRequestDto request);
        Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordWithCodeRequestDto request);
        Task<ApiResponseDto> EditProfileAsync(string userId, EditProfileRequestDto request);
        Task<ApiResponseDto> DeleteAccountAsync(string userId);
    }
}
