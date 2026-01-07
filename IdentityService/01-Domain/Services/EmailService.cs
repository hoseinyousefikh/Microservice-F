using IdentityService._01_Domain.Core.Contracts;
using MailKit.Net.Smtp;
using MimeKit;

namespace IdentityService._01_Domain.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            await SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        }


        public async Task SendPasswordResetEmailAsync(string email, string code)
        {
            // به جای callbackUrl، کد را ارسال می‌کنیم
            await SendEmailAsync(email, "Your Password Reset Code",
                $"Your password reset code is: <h2>{code}</h2>. This code is valid for 1 hour.");
        }

        public async Task SendEmailChangeConfirmationAsync(string newEmail, string callbackUrl)
        {
            await SendEmailAsync(newEmail, "Confirm your new email",
                $"Please confirm your new email address by <a href='{callbackUrl}'>clicking here</a>.");
        }

        public async Task SendAccountDeletionConfirmationAsync(string email)
        {
            await SendEmailAsync(email, "Account Deletion Confirmation",
                "Your account has been successfully deleted. We're sorry to see you go!");
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var from = emailSettings["From"];
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = Convert.ToInt32(emailSettings["SmtpPort"]);
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Identity Service", from));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent to {to} with subject: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}");
                throw;
            }
        }
    }
}
