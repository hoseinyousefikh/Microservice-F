using Catalog_Service.src._01_Domain.Core.Primitives;

namespace Catalog_Service.src._02_Infrastructure.ExternalServices
{
    public interface IPricingServiceClient
    {
        Task<Money> GetProductPriceAsync(int productId, CancellationToken cancellationToken = default);
        Task<Money> GetProductPriceBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<Money> GetProductDiscountPriceAsync(int productId, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductPriceAsync(int productId, Money price, CancellationToken cancellationToken = default);
        Task<bool> ApplyDiscountAsync(int productId, decimal discountPercentage, CancellationToken cancellationToken = default);
        Task<bool> RemoveDiscountAsync(int productId, CancellationToken cancellationToken = default);
        Task<PriceHistory> GetPriceHistoryAsync(int productId, CancellationToken cancellationToken = default);
    }

    public class PriceHistory
    {
        public int ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
