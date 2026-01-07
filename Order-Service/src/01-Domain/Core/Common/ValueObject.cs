namespace Order_Service.src._01_Domain.Core.Common
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract bool Equals(ValueObject other);

        public override bool Equals(object obj)
        {
            return Equals(obj as ValueObject);
        }

        public override int GetHashCode()
        {
            // GetEqualityComponents should be implemented by the derived class
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }
            return left?.Equals(right) != false;
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        /// <summary>
        /// Gets the components of the value object that are used for equality.
        /// </summary>
        /// <returns>An enumerable of the components.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
