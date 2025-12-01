using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Primitives;
using System.Text.RegularExpressions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class SlugService : ISlugService
    {
        private readonly ILogger<SlugService> _logger;

        public SlugService(ILogger<SlugService> logger)
        {
            _logger = logger;
        }

        public async Task<Slug> CreateSlugAsync(string title, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var slug = Slug.Create(title);
            _logger.LogDebug("Created slug '{Slug}' from title '{Title}'", slug.Value, title);
            return await Task.FromResult(slug);
        }

        public async Task<Slug> CreateUniqueSlugAsync(string title, Func<string, Task<bool>> uniquenessChecker, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            if (uniquenessChecker == null)
                throw new ArgumentNullException(nameof(uniquenessChecker));

            var baseSlug = Slug.Create(title);
            var slug = await EnsureUniquenessAsync(baseSlug.Value, uniquenessChecker, cancellationToken);

            _logger.LogDebug("Created unique slug '{Slug}' from title '{Title}'", slug, title);
            return Slug.FromString(slug);
        }

        public async Task<Slug> CreateUniqueSlugForProductAsync(string title, int? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlug = Slug.Create(title);
            var slug = await EnsureUniquenessAsync(baseSlug.Value,
                slugValue => IsUniqueProductSlugAsync(slugValue, excludeProductId, cancellationToken),
                cancellationToken);

            _logger.LogDebug("Created unique product slug '{Slug}' from title '{Title}'", slug, title);
            return Slug.FromString(slug);
        }

        public async Task<Slug> CreateUniqueSlugForCategoryAsync(string title, int? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlug = Slug.Create(title);
            var slug = await EnsureUniquenessAsync(baseSlug.Value,
                slugValue => IsUniqueCategorySlugAsync(slugValue, excludeCategoryId, cancellationToken),
                cancellationToken);

            _logger.LogDebug("Created unique category slug '{Slug}' from title '{Title}'", slug, title);
            return Slug.FromString(slug);
        }

        public async Task<Slug> CreateUniqueSlugForBrandAsync(string title, int? excludeBrandId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var baseSlug = Slug.Create(title);
            var slug = await EnsureUniquenessAsync(baseSlug.Value,
                slugValue => IsUniqueBrandSlugAsync(slugValue, excludeBrandId, cancellationToken),
                cancellationToken);

            _logger.LogDebug("Created unique brand slug '{Slug}' from title '{Title}'", slug, title);
            return Slug.FromString(slug);
        }

        public async Task<bool> IsUniqueProductSlugAsync(string slug, int? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));

            // In a real implementation, you would check against the product repository
            // For now, we'll assume it's unique (you need to inject IProductRepository)
            // return await _productRepository.IsUniqueSlugAsync(Slug.FromString(slug), excludeProductId, cancellationToken);

            // Placeholder implementation
            return await Task.FromResult(true);
        }

        public async Task<bool> IsUniqueCategorySlugAsync(string slug, int? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));

            // In a real implementation, you would check against the category repository
            // return await _categoryRepository.IsUniqueSlugAsync(Slug.FromString(slug), excludeCategoryId, cancellationToken);

            // Placeholder implementation
            return await Task.FromResult(true);
        }

        public async Task<bool> IsUniqueBrandSlugAsync(string slug, int? excludeBrandId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));

            // In a real implementation, you would check against the brand repository
            // return await _brandRepository.IsUniqueSlugAsync(Slug.FromString(slug), excludeBrandId, cancellationToken);

            // Placeholder implementation
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

        public string GenerateSlugFromTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            return Slug.Create(title).Value;
        }

        public bool IsValidSlug(string slug)
        {
            // پیاده‌سازی مستقیم منطق اعتبارسنجی اسلاگ به جای استفاده از متد محافظت شده Slug.IsValidSlug
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            // اسلاگ باید فقط شامل حروف کوچک، اعداد، خط تیره و زیرخط باشد
            if (!Regex.IsMatch(slug, @"^[a-z0-9\-_]+$"))
                return false;

            // اسلاگ نباید با خط تیره یا زیرخط شروع یا تمام شود
            if (slug.StartsWith("-") || slug.EndsWith("-") || slug.StartsWith("_") || slug.EndsWith("_"))
                return false;

            // اسلاگ نباید شامل دو خط تیره یا زیرخط متوالی باشد
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

            // بررسی اینکه آیا اسلاگ پایه منحصر به فرد است
            if (!await uniquenessChecker(slug))
            {
                // اگر منحصر به فرد نیست، یک عدد به انتهای آن اضافه می‌کنیم تا اسلاگ منحصر به فرد پیدا شود
                while (counter < 100) // جلوگیری از حلقه بی‌نهایت
                {
                    var candidate = $"{baseSlug}-{counter}";
                    if (await uniquenessChecker(candidate))
                    {
                        slug = candidate;
                        break;
                    }
                    counter++;
                }

                // اگر پس از 100 تلاش هنوز اسلاگ منحصر به فرد پیدا نشد، استثنا پرتاب می‌کنیم
                if (counter >= 100)
                {
                    throw new InvalidOperationException($"Could not generate a unique slug for '{baseSlug}' after 100 attempts");
                }
            }

            return slug;
        }
    }
}