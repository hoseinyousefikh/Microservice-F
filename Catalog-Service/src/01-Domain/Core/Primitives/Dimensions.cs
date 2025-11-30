namespace Catalog_Service.src._01_Domain.Core.Primitives
{
    public class Dimensions : ValueObject
    {
        public decimal Length { get; }
        public decimal Width { get; }
        public decimal Height { get; }
        public string Unit { get; }

        private Dimensions(decimal length, decimal width, decimal height, string unit)
        {
            Length = length;
            Width = width;
            Height = height;
            Unit = unit;
        }

        public static Dimensions Create(decimal length, decimal width, decimal height, string unit = "cm")
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));

            if (width <= 0)
                throw new ArgumentException("Width must be positive", nameof(width));

            if (height <= 0)
                throw new ArgumentException("Height must be positive", nameof(height));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));

            return new Dimensions(length, width, height, unit);
        }

        public decimal Volume() => Length * Width * Height;

        public Dimensions Scale(decimal factor)
        {
            if (factor <= 0)
                throw new ArgumentException("Scale factor must be positive", nameof(factor));

            return new Dimensions(
                Length * factor,
                Width * factor,
                Height * factor,
                Unit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Length;
            yield return Width;
            yield return Height;
            yield return Unit;
        }

        public override string ToString()
        {
            return $"{Length} x {Width} x {Height} {Unit}";
        }
    }
}
