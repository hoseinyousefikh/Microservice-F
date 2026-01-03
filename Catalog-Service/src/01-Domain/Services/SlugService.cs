using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Primitives;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class SlugService : ISlugService
    {
        private readonly ILogger<SlugService> _logger;
        private readonly IBrandRepository _brandRepository;

        public SlugService(ILogger<SlugService> logger, IBrandRepository brandRepository)
        {
            _logger = logger;
            _brandRepository = brandRepository;
        }

        // تابع کمکی برای تبدیل کاراکترهای فارسی به انگلیسی (اصلاح شده)
        private static string ConvertPersianToEnglish(string input)
        {
            // آرایه‌ها از نوع string تعریف شده‌اند
            var persianChars = new[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹", "ا", "ب", "پ", "ت", "ث", "ج", "چ", "ح", "خ", "د", "ذ", "ر", "ز", "ژ", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن", "و", "ه", "ی", "آ", "ء", "ئ", "ؤ", "إ", "أ" };
            var englishEquivalents = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "p", "t", "s", "j", "ch", "h", "kh", "d", "z", "r", "z", "zh", "s", "sh", "s", "z", "t", "z", "e", "gh", "f", "q", "k", "g", "l", "m", "n", "v", "h", "y", "a", "'", "'", "'", "'", "'", "a" };

            for (int i = 0; i < persianChars.Length; i++)
            {
                input = input.Replace(persianChars[i], englishEquivalents[i]);
            }
            return input;
        }

        public string GenerateSlugFromTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            string slug = ConvertPersianToEnglish(title);

            // حذف کاراکترهای غیرمجاز با استفاده از Regex (روش مدرن و مطمئن)
            slug = Regex.Replace(slug, @"[^a-zA-Z0-9\s-]", ""); // فقط حروف انگلیسی، اعداد، فاصله و خط تیره باقی می‌ماند

            slug = Regex.Replace(slug, @"\s+", "-").Trim(); // فاصله‌ها را به خط تیره تبدیل کرده و فاصله‌های اضافی را حذف می‌کند
            slug = Regex.Replace(slug, @"-+", "-"); // خط تیره‌های متوالی را به یکی تبدیل می‌کند

            return slug.ToLowerInvariant();
        }

        public async Task<Slug> CreateSlugAsync(string title, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var slugValue = GenerateSlugFromTitle(title);
            var slug = Slug.Create(slugValue);
            _logger.LogDebug("Created slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        // *** این متد اصلاح شده است ***
        public async Task<Slug> CreateUniqueSlugAsync(string title, Func<string, Task<bool>> uniquenessChecker, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            if (uniquenessChecker == null)
                throw new ArgumentNullException(nameof(uniquenessChecker));

            var baseSlugValue = GenerateSlugFromTitle(title);
            var uniqueSlugValue = await EnsureUniquenessAsync(baseSlugValue, uniquenessChecker, cancellationToken);

            // فقط یک بار Slug.Create را فراخوانی کنید و نتیجه را مستقیماً برگردانید
            var slug = Slug.Create(uniqueSlugValue);

            _logger.LogDebug("Created unique slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        // *** این متد اصلاح شده است ***
        public async Task<Slug> CreateUniqueSlugForProductAsync(string title, int? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlugValue = GenerateSlugFromTitle(title);
            var uniqueSlugValue = await EnsureUniquenessAsync(baseSlugValue,
                slugValue => IsUniqueProductSlugAsync(slugValue, excludeProductId, cancellationToken),
                cancellationToken);

            var slug = Slug.Create(uniqueSlugValue);

            _logger.LogDebug("Created unique product slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        // *** این متد اصلاح شده است ***
        public async Task<Slug> CreateUniqueSlugForCategoryAsync(string title, int? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlugValue = GenerateSlugFromTitle(title);
            var uniqueSlugValue = await EnsureUniquenessAsync(baseSlugValue,
                slugValue => IsUniqueCategorySlugAsync(slugValue, excludeCategoryId, cancellationToken),
                cancellationToken);

            var slug = Slug.Create(uniqueSlugValue);

            _logger.LogDebug("Created unique category slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        // *** این متد اصلاح شده است ***
        public async Task<Slug> CreateUniqueSlugForBrandAsync(string title, int? excludeBrandId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlugValue = GenerateSlugFromTitle(title);
            var uniqueSlugValue = await EnsureUniquenessAsync(baseSlugValue,
                slugValue => IsUniqueBrandSlugAsync(slugValue, excludeBrandId, cancellationToken),
                cancellationToken);

            var slug = Slug.Create(uniqueSlugValue);

            _logger.LogDebug("Created unique brand slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        public async Task<bool> IsUniqueBrandSlugAsync(string slug, int? excludeBrandId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));

            // ابتدا string را به یک شیء Slug تبدیل کنید
            var slugObject = Slug.Create(slug);

            // سپس شیء Slug را به ریپازیتوری بدهید
            var existingBrand = await _brandRepository.GetBySlugAsync(slugObject, cancellationToken);

            var isUnique = existingBrand == null || (excludeBrandId.HasValue && existingBrand.Id == excludeBrandId.Value);
            return isUnique;
        }
        public async Task<bool> IsUniqueProductSlugAsync(string slug, int? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));
            return await Task.FromResult(true);
        }

        public async Task<bool> IsUniqueCategorySlugAsync(string slug, int? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));
            return await Task.FromResult(true);
        }

        public async Task<Slug> UpdateProductSlugAsync(int productId, string title, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var slug = await CreateUniqueSlugForProductAsync(title, productId, cancellationToken);
            _logger.LogInformation("Updated slug for product with ID {ProductId} to '{Slug}'", productId, slug.Value);
            return slug;
        }

        public async Task<Slug> UpdateCategorySlugAsync(int categoryId, string title, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID", nameof(categoryId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var slug = await CreateUniqueSlugForCategoryAsync(title, categoryId, cancellationToken);
            _logger.LogInformation("Updated slug for category with ID {CategoryId} to '{Slug}'", categoryId, slug.Value);
            return slug;
        }

        public async Task<Slug> UpdateBrandSlugAsync(int brandId, string title, CancellationToken cancellationToken = default)
        {
            if (brandId <= 0)
                throw new ArgumentException("Invalid brand ID", nameof(brandId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var slug = await CreateUniqueSlugForBrandAsync(title, brandId, cancellationToken);
            _logger.LogInformation("Updated slug for brand with ID {BrandId} to '{Slug}'", brandId, slug.Value);
            return slug;
        }

        public bool IsValidSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            if (!Regex.IsMatch(slug, @"^[a-z0-9\-_]+$"))
                return false;

            if (slug.StartsWith("-") || slug.EndsWith("-") || slug.StartsWith("_") || slug.EndsWith("_"))
                return false;

            if (slug.Contains("--") || slug.Contains("__") || slug.Contains("-_") || slug.Contains("_-"))
                return false;

            return true;
        }

        public async Task<string> EnsureUniquenessAsync(string baseSlug, Func<string, Task<bool>> uniquenessChecker, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(baseSlug))
                throw new ArgumentException("Base slug is required", nameof(baseSlug));

            if (uniquenessChecker == null)
                throw new ArgumentNullException(nameof(uniquenessChecker));

            var slug = baseSlug;
            var counter = 1;

            if (!await uniquenessChecker(slug))
            {
                while (counter < 100)
                {
                    var candidate = $"{baseSlug}-{counter}";
                    if (await uniquenessChecker(candidate))
                    {
                        slug = candidate;
                        break;
                    }
                    counter++;
                }

                if (counter >= 100)
                {
                    throw new InvalidOperationException($"Could not generate a unique slug for '{baseSlug}' after 100 attempts");
                }
            }

            return slug;
        }
    }
}