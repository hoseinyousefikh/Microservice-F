using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.UserManagement
{
    public class ChangeUsernameRequestDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string NewUsername { get; set; }

        [Required]
        public string Password { get; set; }
    }
}