using IdentityService._02_Infrastructures.Data.Repositories;

namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task<int> CommitAsync();
    }
}
