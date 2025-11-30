namespace Catalog_Service.src._01_Domain.Core.Primitives
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

        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            return new Money(amount, currency);
        }

        public static Money Zero(string currency) => new(0, currency);

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            if (Amount < other.Amount)
                throw new InvalidOperationException("Insufficient funds");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            if (multiplier < 0)
                throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

            return new Money(Amount * multiplier, Currency);
        }

        public Money Divide(decimal divisor)
        {
            if (divisor <= 0)
                throw new ArgumentException("Divisor must be positive", nameof(divisor));

            return new Money(Amount / divisor, Currency);
        }

        public bool IsZero() => Amount == 0;

        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");

            return Amount > other.Amount;
        }

        public bool IsLessThan(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");

            return Amount < other.Amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}
