using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.Admin
{
    public class AdminUpdateUserRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? ProfileImageUrl { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Roles { get; set; } = new();
    }
}