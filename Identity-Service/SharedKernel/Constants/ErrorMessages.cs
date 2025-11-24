public static class ErrorMessages
{
    // Authentication Errors
    public const string InvalidCredentials = "Invalid username or password.";
    public const string UserNotFound = "User not found.";
    public const string EmailNotConfirmed = "Email address not confirmed.";
    public const string AccountLocked = "Account is locked due to multiple failed login attempts.";
    public const string InvalidToken = "Invalid or expired token.";

    // User Management Errors
    public const string DuplicateEmail = "Email address already in use.";
    public const string DuplicateUsername = "Username already taken.";
    public const string WeakPassword = "Password does not meet complexity requirements.";
    public const string InvalidCurrentPassword = "Current password is incorrect.";
    public const string UnauthorizedOperation = "You are not authorized to perform this action.";

    // Role Management Errors
    public const string RoleNotFound = "Role not found.";
    public const string RoleAlreadyAssigned = "User already has this role.";
    public const string RoleNotAssigned = "User does not have this role.";

    // Validation Errors
    public const string RequiredField = "{0} is required.";
    public const string InvalidFormat = "{0} is in invalid format.";
    public const string ExceededMaxLength = "{0} exceeds maximum length of {1} characters.";

    // System Errors
    public const string DatabaseError = "An error occurred while accessing the database.";
    public const string ExternalServiceError = "An error occurred while communicating with an external service.";
    public const string UnexpectedError = "An unexpected error occurred. Please try again later.";
}