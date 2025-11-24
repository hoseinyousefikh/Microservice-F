
using Identity_Service.Domain.Enums;
using SharedKernel.Domain.Primitives;

namespace Identity_Service.Domain.Enums
{
    public class PermissionType : Enumeration
    {
        // User Management
        public static readonly PermissionType CreateUser = new(1, nameof(CreateUser));
        public static readonly PermissionType ReadUser = new(2, nameof(ReadUser));
        public static readonly PermissionType UpdateUser = new(3, nameof(UpdateUser));
        public static readonly PermissionType DeleteUser = new(4, nameof(DeleteUser));

        // Role Management
        public static readonly PermissionType CreateRole = new(5, nameof(CreateRole));
        public static readonly PermissionType ReadRole = new(6, nameof(ReadRole));
        public static readonly PermissionType UpdateRole = new(7, nameof(UpdateRole));
        public static readonly PermissionType DeleteRole = new(8, nameof(DeleteRole));
        public static readonly PermissionType AssignRole = new(9, nameof(AssignRole));
        public static readonly PermissionType RevokeRole = new(10, nameof(RevokeRole));

        // System Administration
        public static readonly PermissionType ManageSystemSettings = new(11, nameof(ManageSystemSettings));
        public static readonly PermissionType ViewAuditLogs = new(12, nameof(ViewAuditLogs));

        // Profile Management
        public static readonly PermissionType UpdateOwnProfile = new(13, nameof(UpdateOwnProfile));
        public static readonly PermissionType ChangeOwnPassword = new(14, nameof(ChangeOwnPassword));
        public static readonly PermissionType DeleteOwnAccount = new(15, nameof(DeleteOwnAccount));

        private PermissionType(int id, string name) : base(id, name)
        {
        }
    }
}
