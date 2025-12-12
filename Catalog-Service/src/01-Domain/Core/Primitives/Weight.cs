namespace Catalog_Service.src._01_Domain.Core.Primitives
{
    public class Weight : ValueObject
    {
        public decimal Value { get; }
        public string Unit { get; }

        private Weight(decimal value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Weight Create(decimal value, string unit)
        {
            if (value <= 0)
                throw new ArgumentException("Weight must be positive", nameof(value));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));

            return new Weight(value, unit);
        }

        public Weight Add(Weight other)
        {
            if (Unit != other.Unit)
                throw new InvalidOperationException("Cannot add weights with different units");

            return new Weight(Value + other.Value, Unit);
        }

        public Weight Subtract(Weight other)
        {
            if (Unit != other.Unit)
                throw new InvalidOperationException("Cannot subtract weights with different units");

            if (Value < other.Value)
                throw new InvalidOperationException("Resulting weight would be negative");

            return new Weight(Value - other.Value, Unit);
        }

        public Weight ConvertTo(string newUnit)
        {
            if (string.IsNullOrWhiteSpace(newUnit))
                throw new ArgumentException("Unit cannot be empty", nameof(newUnit));

            if (Unit == newUnit)
                return this;

            decimal convertedValue = ConvertValue(Value, Unit, newUnit);
            return new Weight(convertedValue, newUnit);
        }

        private decimal ConvertValue(decimal value, string fromUnit, string toUnit)
        {
            // Simplified conversion - in a real app, you'd have a more comprehensive conversion table
            if (fromUnit == "g" && toUnit == "kg")
                return value / 1000;
            if (fromUnit == "kg" && toUnit == "g")
                return value * 1000;
            if (fromUnit == "lb" && toUnit == "oz")
                return value * 16;
            if (fromUnit == "oz" && toUnit == "lb")
                return value / 16;

            throw new InvalidOperationException($"Conversion from {fromUnit} to {toUnit} is not supported");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}
