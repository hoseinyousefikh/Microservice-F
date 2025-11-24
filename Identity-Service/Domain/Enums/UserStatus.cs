using Identity_Service.Domain.Enums;
using SharedKernel.Domain.Primitives;
using System.Linq;
using System.Reflection;

namespace Identity_Service.Domain.Enums
{
    public class UserStatus : Enumeration
    {
        public static readonly UserStatus Active = new(1, nameof(Active));
        public static readonly UserStatus Inactive = new(2, nameof(Inactive));
        public static readonly UserStatus Locked = new(3, nameof(Locked));
        public static readonly UserStatus PendingVerification = new(4, nameof(PendingVerification));

        private UserStatus(int id, string name) : base(id, name)
        {
        }

        public static UserStatus FromId(int id)
        {
            var userStatusFields = typeof(UserStatus)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(UserStatus));

            var field = userStatusFields.FirstOrDefault(f => ((UserStatus)f.GetValue(null)).Id == id);

            if (field != null)
            {
                return (UserStatus)field.GetValue(null);
            }

            throw new ArgumentException($"Invalid {nameof(UserStatus)} ID: {id}");
        }
    }
}