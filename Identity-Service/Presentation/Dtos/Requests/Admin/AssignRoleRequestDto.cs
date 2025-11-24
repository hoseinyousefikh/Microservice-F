using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.Admin
{
    public class AssignRoleRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }
    }
}