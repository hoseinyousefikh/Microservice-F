namespace IdentityService._03_EndPoints.DTOs.Requests
{
    public class ResetPasswordWithCodeRequestDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
