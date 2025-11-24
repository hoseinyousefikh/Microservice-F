namespace Identity_Service.Infrastructure.Persistence.Entities
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsUsed => UsedAt.HasValue;
        public bool IsValid => !IsUsed && !IsExpired;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
