using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.UserManagement
{
    public class ChangeEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}