using Microsoft.AspNetCore.Identity;

namespace IdentityService._01_Domain.Core.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
