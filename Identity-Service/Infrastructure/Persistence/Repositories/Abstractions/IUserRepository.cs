

using Identity_Service.Infrastructure.Persistence.Entities;
using SharedKernel.Application.Abstractions;
using SharedKernel.Common.Primitives;

namespace Identity_Service.Infrastructure.Persistence.Repositories.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
        Task<PagedList<User>> GetFilteredUsersAsync(
            string? searchTerm = null,
            int? status = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
