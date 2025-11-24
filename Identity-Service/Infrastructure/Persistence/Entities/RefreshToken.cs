using System;

namespace Identity_Service.Infrastructure.Persistence.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; }

        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}