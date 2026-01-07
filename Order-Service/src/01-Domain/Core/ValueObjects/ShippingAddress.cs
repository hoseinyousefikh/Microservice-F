using Order_Service.src._01_Domain.Core.Common;

namespace Order_Service.src._01_Domain.Core.ValueObjects
{
    public class ShippingAddress : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public string Country { get; }

        private ShippingAddress(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public static ShippingAddress Create(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street is required.", nameof(street));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.", nameof(postalCode));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.", nameof(country));

            return new ShippingAddress(street, city, state, postalCode, country);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return PostalCode;
            yield return Country;
        }

        public override bool Equals(ValueObject other)
        {
            if (other is not ShippingAddress otherAddress)
                return false;

            return Street == otherAddress.Street &&
                   City == otherAddress.City &&
                   State == otherAddress.State &&
                   PostalCode == otherAddress.PostalCode &&
                   Country == otherAddress.Country;
        }
    }
}
