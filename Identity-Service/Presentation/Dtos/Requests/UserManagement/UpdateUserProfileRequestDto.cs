using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.UserManagement
{
    public class UpdateUserProfileRequestDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? ProfileImageUrl { get; set; }
    }
}