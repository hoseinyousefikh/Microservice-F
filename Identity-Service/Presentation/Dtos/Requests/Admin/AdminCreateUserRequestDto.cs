using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.Admin
{
    public class AdminCreateUserRequestDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Roles { get; set; } = new();
    }
}