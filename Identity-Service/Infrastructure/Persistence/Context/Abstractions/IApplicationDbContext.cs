

using Identity_Service.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Application.Abstractions;

namespace Identity_Service.Infrastructure.Persistence.Context.Abstractions
{
    public interface IApplicationDbContext : IUnitOfWork
    {
        // DbSet ها برای تمام موجودیت‌ها
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<PasswordResetToken> PasswordResetTokens { get; }

        // متدهای کمکی برای مدیریت موجودیت‌ها
        Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;

        // پشتیبانی از Change Tracker
        void ChangeTrackerClear();
    }
}
