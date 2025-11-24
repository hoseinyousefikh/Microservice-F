namespace IdentityService._01_Domain.Core.Contracts
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string callbackUrl);
        Task SendPasswordResetEmailAsync(string email, string callbackUrl);
        Task SendEmailChangeConfirmationAsync(string newEmail, string callbackUrl);
        Task SendAccountDeletionConfirmationAsync(string email);
    }
}
