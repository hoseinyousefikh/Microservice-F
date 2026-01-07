using Order_Service.src._01_Domain.Core.Common;

namespace Order_Service.src._01_Domain.Core.Entities
{
    public class Basket : BaseEntity
    {
        public string UserId { get; private set; }

        private readonly List<BasketItem> _items = new();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        // Parameterless constructor for EF Core
        private Basket() : base() { }

        public Basket(Guid id, string userId) : base(id)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public void AddItem(BasketItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
            }
            else
            {
                _items.Add(item);
            }
            UpdateTimestamp();
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                UpdateTimestamp();
            }
        }

        public void Clear()
        {
            _items.Clear();
            UpdateTimestamp();
        }
    }
}
