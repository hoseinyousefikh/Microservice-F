namespace Catalog_Service.src._01_Domain.Core.Primitives
{
    public interface IDomainEvent
    {
        /// <summary>
        /// شناسه منحصر به فرد رویداد
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// زمان وقوع رویداد
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// نوع رویداد
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// نسخه رویداد برای مدیریت تغییرات آینده
        /// </summary>
        int Version { get; }
    }

    /// <summary>
    /// کلاس پایه انتزاعی برای پیاده‌سازی رویدادهای دامنه
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime Timestamp { get; }
        public string EventType { get; }
        public int Version { get; }

        protected DomainEventBase(int version = 1)
        {
            EventId = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            EventType = GetType().Name;
            Version = version;
        }
    }
}
