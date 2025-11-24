namespace Identity_Service.Presentation.Dtos.Responses.Authentication
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Message { get; set; } = "Registration successful. Please check your email to confirm your account.";
    }
}