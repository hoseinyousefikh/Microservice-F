

using Identity_Service.Domain.Entities.ValueObjects;
using Identity_Service.Domain.Enums;
using Identity_Service.Infrastructure.Persistence.Entities;
using SharedKernel.Domain.Primitives;

namespace Identity_Service.Domain.Entities
{
    public class User : Entity
    {
        public string Username { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public string PasswordHash { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public bool PhoneNumberConfirmed { get; private set; }
        public string? ProfileImageUrl { get; private set; }

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private readonly List<PasswordResetToken> _passwordResetTokens = new();
        public IReadOnlyCollection<PasswordResetToken> PasswordResetTokens => _passwordResetTokens.AsReadOnly();

        private User() : base(Guid.NewGuid()) { }

        public User(
            string username,
            Email email,
            string passwordHash,
            string? firstName = null,
            string? lastName = null,
            PhoneNumber? phoneNumber = null) : base(Guid.NewGuid())
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Status = UserStatus.Active;
            EmailConfirmed = false;
            PhoneNumberConfirmed = false;
        }

        public void UpdateProfile(
            string? firstName = null,
            string? lastName = null,
            PhoneNumber? phoneNumber = null,
            string? profileImageUrl = null)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ProfileImageUrl = profileImageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeEmail(Email newEmail)
        {
            Email = newEmail;
            EmailConfirmed = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeUsername(string newUsername)
        {
            Username = newUsername;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ConfirmPhoneNumber()
        {
            PhoneNumberConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = UserStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Lock()
        {
            Status = UserStatus.Locked;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _refreshTokens.Add(refreshToken);
        }

        public void RemoveRefreshToken(RefreshToken refreshToken)
        {
            _refreshTokens.Remove(refreshToken);
        }

        public void AddPasswordResetToken(PasswordResetToken token)
        {
            _passwordResetTokens.Add(token);
        }

        public void RemovePasswordResetToken(PasswordResetToken token)
        {
            _passwordResetTokens.Remove(token);
        }

        public void AddRole(UserRole userRole)
        {
            _userRoles.Add(userRole);
        }

        public void RemoveRole(UserRole userRole)
        {
            _userRoles.Remove(userRole);
        }
    }
}
