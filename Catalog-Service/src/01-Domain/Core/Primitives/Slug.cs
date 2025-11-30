using System.Text.RegularExpressions;

namespace Catalog_Service.src._01_Domain.Core.Primitives
{
    public class Slug : ValueObject
    {
        public string Value { get; }

        private Slug(string value)
        {
            Value = value;
        }

        public static Slug Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Slug title cannot be empty", nameof(title));

            string slug = GenerateSlug(title);
            return new Slug(slug);
        }

        public static Slug FromString(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug cannot be empty", nameof(slug));

            if (!IsValidSlug(slug))
                throw new ArgumentException("Invalid slug format", nameof(slug));

            return new Slug(slug);
        }

        private static string GenerateSlug(string title)
        {
            string slug = title.ToLowerInvariant();

            // Remove all accents
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Replace(" ", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"[-]{2,}", "-");

            return slug;
        }

        private static bool IsValidSlug(string slug)
        {
            return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
