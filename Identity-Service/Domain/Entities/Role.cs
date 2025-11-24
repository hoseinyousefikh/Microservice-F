

using Identity_Service.Infrastructure.Persistence.Entities;
using SharedKernel.Domain.Primitives;

namespace Identity_Service.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role() : base(Guid.NewGuid()) { }

        public Role(string name, string? description = null) : base(Guid.NewGuid())
        {
            Name = name;
            Description = description;
        }

        public void UpdateDetails(string? description = null)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPermission(RolePermission rolePermission)
        {
            _rolePermissions.Add(rolePermission);
        }

        public void RemovePermission(RolePermission rolePermission)
        {
            _rolePermissions.Remove(rolePermission);
        }

        public void AddUser(UserRole userRole)
        {
            _userRoles.Add(userRole);
        }

        public void RemoveUser(UserRole userRole)
        {
            _userRoles.Remove(userRole);
        }
    }
}
