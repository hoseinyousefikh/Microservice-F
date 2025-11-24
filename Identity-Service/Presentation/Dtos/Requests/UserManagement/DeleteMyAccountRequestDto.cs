using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.UserManagement
{
    public class DeleteMyAccountRequestDto
    {
        [Required]
        public string Password { get; set; }
    }
}