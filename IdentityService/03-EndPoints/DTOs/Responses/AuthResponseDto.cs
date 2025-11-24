namespace IdentityService._03_EndPoints.DTOs.Responses
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool RequiresEmailConfirmation { get; set; }
    }
}
