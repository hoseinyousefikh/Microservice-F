using System.ComponentModel.DataAnnotations;

namespace IdentityService._03_EndPoints.DTOs.Requests
{
    public class EditProfileRequestDto
    {
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }
    }
}
