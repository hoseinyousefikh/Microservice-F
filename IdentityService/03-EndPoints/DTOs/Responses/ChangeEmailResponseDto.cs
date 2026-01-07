namespace IdentityService._03_EndPoints.DTOs.Responses
{
    public class ChangeEmailResponseDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string NewEmail { get; set; }
    }
}
