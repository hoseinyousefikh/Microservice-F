using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src.CrossCutting.Exceptions;

namespace Catalog_Service.src._01_Domain.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISlugService _slugService;
        private readonly ILogger<BrandService> _logger;

        public BrandService(
            IBrandRepository brandRepository,
            IProductRepository productRepository,
            ISlugService slugService,
            ILogger<BrandService> logger)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<Brand?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);
            if (brand == null)
            {
                _logger.LogWarning("Brand with ID {BrandId} not found", id);
                return null;
            }
            return brand;
        }

        public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetBySlugAsync(Slug.FromString(slug), cancellationToken);
            if (brand == null)
            {
                _logger.LogWarning("Brand with slug {BrandSlug} not found", slug);
                return null;
            }
            return brand;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetActiveBrandsAsync(CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetActiveBrandsAsync(cancellationToken);
        }

        public async Task<Brand> CreateAsync(string name, string description, string createdByUserId, string? logoUrl = null, string? websiteUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(createdByUserId))
                throw new ArgumentException("CreatedByUserId is required", nameof(createdByUserId));

            var brand = new Brand(name, description, createdByUserId, logoUrl, websiteUrl, metaTitle, metaDescription);

            var slug = await _slugService.CreateUniqueSlugForBrandAsync(
                title: name,
                cancellationToken: cancellationToken
            ); brand.SetSlug(slug);

            brand = await _brandRepository.AddAsync(brand, cancellationToken);
            await _brandRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new brand with ID {BrandId} and name {BrandName}", brand.Id, brand.Name);
            return brand;
        }

        public async Task UpdateAsync(int id, string name, string description, string? logoUrl = null, string? websiteUrl = null, string? metaTitle = null, string? metaDescription = null, CancellationToken cancellationToken = default)
        {
            var brand = await GetByIdAsync(id, cancellationToken);
            if (brand == null)
            {
                throw new NotFoundException($"Brand with ID {id} not found for update.");
            }

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name is required", nameof(name));

            brand.UpdateDetails(name, description, logoUrl, websiteUrl, metaTitle, metaDescription);

            if (brand.Name != name)
            {
                var slug = await _slugService.CreateUniqueSlugForBrandAsync(name, id, cancellationToken);
                brand.SetSlug(slug);
            }

            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated brand with ID {BrandId}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var brand = await GetByIdAsync(id, cancellationToken);
            if (brand == null)
            {
                throw new NotFoundException($"Brand with ID {id} not found for delete.");
            }

            if (await _brandRepository.HasProductsAsync(id, cancellationToken))
                throw new BusinessRuleException("Cannot delete brand that has products");

            _brandRepository.Remove(brand);
            await _brandRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted brand with ID {BrandId}", id);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _brandRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<(IEnumerable<Brand> Brands, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string searchTerm = null, bool onlyActive = true, string sortBy = null, bool sortAscending = true, CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, onlyActive, sortBy, sortAscending, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetProductsAsync(brandId, cancellationToken);
        }

        public async Task<int> GetProductsCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetProductsCountAsync(brandId, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.HasProductsAsync(brandId, cancellationToken);
        }

        public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(id, cancellationToken))
            {
                throw new NotFoundException($"Brand with ID {id} not found for activation.");
            }
            await _brandRepository.ActivateAsync(id, cancellationToken);
            _logger.LogInformation("Activated brand with ID {BrandId}", id);
        }

        public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(id, cancellationToken))
            {
                throw new NotFoundException($"Brand with ID {id} not found for deactivation.");
            }
            await _brandRepository.DeactivateAsync(id, cancellationToken);
            _logger.LogInformation("Deactivated brand with ID {BrandId}", id);
        }

        public async Task SetSlugAsync(int id, string title, CancellationToken cancellationToken = default)
        {
            var brand = await GetByIdAsync(id, cancellationToken);
            if (brand == null)
            {
                throw new NotFoundException($"Brand with ID {id} not found for slug update.");
            }
            var slug = await _slugService.CreateUniqueSlugForBrandAsync(title, id, cancellationToken);
            brand.SetSlug(slug);
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated slug for brand with ID {BrandId}", id);
        }

        public async Task<decimal> GetAveragePriceAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetAveragePriceAsync(brandId, cancellationToken);
        }

        public async Task<int> GetTotalViewCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetTotalViewCountAsync(brandId, cancellationToken);
        }

        public async Task<int> GetTotalReviewsCountAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetTotalReviewsCountAsync(brandId, cancellationToken);
        }

        public async Task<double> GetAverageRatingAsync(int brandId, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);
            if (brand == null)
                throw new NotFoundException($"Brand with ID {brandId} not found");

            return await _brandRepository.GetAverageRatingAsync(brandId, cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsByProductsCountAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetTopBrandsByProductsCountAsync(count, cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsByRatingAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetTopBrandsByRatingAsync(count, cancellationToken);
        }

        public async Task<IEnumerable<Brand>> GetTopBrandsBySalesAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _brandRepository.GetTopBrandsBySalesAsync(count, cancellationToken);
        }
    }
}