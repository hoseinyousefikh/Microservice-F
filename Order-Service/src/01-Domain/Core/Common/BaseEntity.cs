namespace Order_Service.src._01_Domain.Core.Common
{
    public abstract class BaseEntity : IEquatable<BaseEntity>
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? ModifiedAt { get; protected set; }

        protected BaseEntity(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            }
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        // Parameterless constructor for EF Core
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateTimestamp()
        {
            ModifiedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        public bool Equals(BaseEntity other)
        {
            return other is not null && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !(left == right);
        }
    }
}
