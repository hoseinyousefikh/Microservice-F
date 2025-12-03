namespace Catalog_Service.src._02_Infrastructure.ExternalServices
{
    public interface IInventoryServiceClient
    {
        Task<InventoryStatus> GetInventoryStatusAsync(int productId, CancellationToken cancellationToken = default);
        Task<InventoryStatus> GetInventoryStatusBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> UpdateInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default);
        Task<bool> ReserveInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default);
        Task<bool> ReleaseInventoryAsync(int productId, int quantity, CancellationToken cancellationToken = default);
        Task<bool> CheckInventoryAvailabilityAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    }

    public enum InventoryStatus
    {
        InStock,
        LowStock,
        OutOfStock,
        Discontinued
    }
}
