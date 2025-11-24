using Ardalis.Specification;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Enums;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Identity_Service.Domain.Specifications
{
    public static class UserSpecifications
    {
        public static Expression<Func<User, bool>> ById(Guid id)
        {
            return user => user.Id == id;
        }

        public static Expression<Func<User, bool>> ByUsername(string username)
        {
            return user => user.Username == username;
        }

        public static Expression<Func<User, bool>> ByEmail(string email)
        {
            return user => user.Email.Value == email;
        }

        public static Expression<Func<User, bool>> ByStatus(UserStatus status)
        {
            return user => user.Status == status;
        }

        public static Expression<Func<User, bool>> ActiveUsers()
        {
            return user => user.Status == UserStatus.Active;
        }

        public static Expression<Func<User, bool>> WithRole(string roleName)
        {
            return user => user.UserRoles.Any(ur => ur.Role.Name == roleName);
        }

        public static Expression<Func<User, bool>> CreatedAfter(DateTime date)
        {
            return user => user.CreatedAt >= date;
        }

        public static Expression<Func<User, bool>> CreatedBefore(DateTime date)
        {
            return user => user.CreatedAt <= date;
        }

        public static Expression<Func<User, bool>> WithUnconfirmedEmail()
        {
            return user => !user.EmailConfirmed;
        }

        public static Expression<Func<User, bool>> WithUnconfirmedPhoneNumber()
        {
            return user => !user.PhoneNumberConfirmed;
        }

        public static Expression<Func<User, bool>> Search(string searchTerm)
        {
            return user =>
                user.Username.Contains(searchTerm) ||
                user.Email.Value.Contains(searchTerm) ||
                (user.FirstName != null && user.FirstName.Contains(searchTerm)) ||
                (user.LastName != null && user.LastName.Contains(searchTerm));
        }
    }
}
