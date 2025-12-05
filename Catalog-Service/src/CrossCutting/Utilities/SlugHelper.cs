namespace Catalog_Service.src.CrossCutting.Utilities
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            var s = phrase.ToLowerInvariant();
            s = s.Replace(" ", "-");
            s = s.Replace("_", "-");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\s-]", "");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim();
            s = s.Replace(" ", "-");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"-+", "-");

            return s;
        }

        public static string EnsureUniqueSlug(string baseSlug, Func<string, Task<bool>> uniquenessChecker)
        {
            if (string.IsNullOrWhiteSpace(baseSlug))
                return string.Empty;

            var slug = baseSlug;
            var counter = 1;

            while (!uniquenessChecker(slug).Result)
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
