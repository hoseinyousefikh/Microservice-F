using Order_Service.src._01_Domain.Core.Common;

namespace Order_Service.src._01_Domain.Core.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Zero(string currency = "USD") => new Money(0, currency);

        public static Money FromDecimal(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Money amount cannot be negative.", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty.", nameof(currency));

            return new Money(amount, currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override bool Equals(ValueObject other)
        {
            if (other is not Money otherMoney)
                return false;

            return Amount == otherMoney.Amount && Currency == otherMoney.Currency;
        }

        public static Money operator +(Money left, Money right)
        {
            if (left is null || right is null)
                throw new ArgumentNullException("Operands cannot be null.");
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies.");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator *(Money money, int multiplier)
        {
            if (money is null)
                throw new ArgumentNullException(nameof(money));
            if (multiplier < 0)
                throw new ArgumentException("Multiplier cannot be negative.", nameof(multiplier));

            return new Money(money.Amount * multiplier, money.Currency);
        }

        public static bool operator <(Money left, Money right)
        {
            if (left is null || right is null) return false;
            if (left.Currency != right.Currency) throw new InvalidOperationException("Cannot compare money with different currencies.");
            return left.Amount < right.Amount;
        }

        public static bool operator >(Money left, Money right)
        {
            if (left is null || right is null) return false;
            if (left.Currency != right.Currency) throw new InvalidOperationException("Cannot compare money with different currencies.");
            return left.Amount > right.Amount;
        }
    }
}
