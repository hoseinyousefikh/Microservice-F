using Order_Service.src._01_Domain.Core.Common;

namespace Order_Service.src._01_Domain.Core.ValueObjects
{
    public class OrderNumber : ValueObject
    {
        public string Value { get; }

        private OrderNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Generates a new, unique order number.
        /// Format: ORD-YYYYMMDD-XXXXX
        /// </summary>
        public static OrderNumber GenerateUnique()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..8].ToUpper(); // Take 8 chars from GUID
            var orderNumber = $"ORD-{datePart}-{randomPart}";
            return new OrderNumber(orderNumber);
        }

        /// <summary>
        /// Creates an OrderNumber from an existing string value.
        /// Useful for re-hydrating from the database.
        /// </summary>
        public static OrderNumber FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Order number cannot be empty.", nameof(value));

            // Add more validation for the format if needed
            if (!value.StartsWith("ORD-"))
                throw new ArgumentException("Invalid order number format.", nameof(value));

            return new OrderNumber(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override bool Equals(ValueObject other)
        {
            return other is OrderNumber otherOrder && Value == otherOrder.Value;
        }

        public override string ToString() => Value;
    }
}
