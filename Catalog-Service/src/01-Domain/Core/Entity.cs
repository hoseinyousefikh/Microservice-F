namespace Catalog_Service.src._01_Domain.Core
{
    public abstract class Entity
    {
        // شناسه منحصر به فرد موجودیت
        public int Id { get; protected set; }

        // سازنده برای مقداردهی اولیه شناسه
        protected Entity(int id)
        {
            Id = id;
        }

        // سازنده بدون پارامتر برای EF Core
        protected Entity()
        {
        }

        // بازنویسی متد Equals برای مقایسه موجودیت‌ها بر اساس شناسه و نوع
        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            // اگر موجودیت‌ها گذرا باشند (شناسه صفر) و نمونه‌های متفاوتی باشند، برابر نیستند
            if (Id == 0 || other.Id == 0)
                return false;

            return Id == other.Id;
        }

        // بازنویسی متد GetHashCode برای هش کردن مناسب در مجموعه‌ها
        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }

        // عملگر برابری ==
        public static bool operator ==(Entity left, Entity right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        // عملگر نابرابری !=
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
