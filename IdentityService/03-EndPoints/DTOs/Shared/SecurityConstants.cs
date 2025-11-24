namespace IdentityService._03_EndPoints.DTOs.Shared
{
    public static class SecurityConstants
    {
        public const int DefaultPasswordLength = 12;
        public const int MaxPasswordLength = 100;
        public const int DefaultTokenExpiryMinutes = 15;
        public const int DefaultRefreshTokenExpiryDays = 7;
        public const int MaxLoginAttempts = 5;
        public const int LockoutDurationMinutes = 15;
        public const int RateLimitPerMinute = 100;
        public const int MaxUserNameLength = 100;
        public const int MaxEmailLength = 256;
    }
}
