using Order_Service.src._01_Domain.Core.Entities;

namespace Order_Service.src._01_Domain.Core.Interfaces.Repositories
{
    public interface IBasketRepository
    {
        Task<Basket?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Basket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Basket basket, CancellationToken cancellationToken = default);
        void Update(Basket basket);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
