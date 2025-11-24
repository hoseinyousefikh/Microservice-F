using System.ComponentModel.DataAnnotations;

namespace IdentityService._03_EndPoints.DTOs.Requests
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
