using System.ComponentModel.DataAnnotations;

namespace IdentityService._03_EndPoints.DTOs.Requests
{
    public class ChangeEmailRequestDto
    {
        [Required(ErrorMessage = "New email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
