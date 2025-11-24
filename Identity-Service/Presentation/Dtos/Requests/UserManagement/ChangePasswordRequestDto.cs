using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.UserManagement
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}