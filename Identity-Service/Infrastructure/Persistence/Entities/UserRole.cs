using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Identity_Service.Infrastructure.Persistence.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public DateTime AssignedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}