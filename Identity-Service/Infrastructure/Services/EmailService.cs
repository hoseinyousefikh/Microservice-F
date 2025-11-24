using Identity_Service.Application.Interfaces;
using Identity_Service.Infrastructure.ExternalServices;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Identity_Service.Infrastructure.Services
{
    public class EmailService : IEmailServices
    {
        private readonly SendGridClient _sendGridClient;
        private readonly EmailSettings _emailSettings;

        public EmailService(SendGridClient sendGridClient, IOptions<EmailSettings> emailSettings)
        {
            _sendGridClient = sendGridClient;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await _sendGridClient.SendEmailAsync(
                from: _emailSettings.FromEmail,
                fromName: _emailSettings.FromName,
                to: to,
                subject: subject,
                body: body,
                isHtml: isHtml
            );
        }

        public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
        {
            var subject = "Confirm your email";
            var body = $@"
                <h2>Welcome to MarketHub!</h2>
                <p>Please confirm your email by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you didn't create an account, please ignore this email.</p>
            ";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Reset your password";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>Please reset your password by clicking the link below:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request a password reset, please ignore this email.</p>
            ";

            await SendEmailAsync(to, subject, body, true);
        }
    }

    public class EmailSettings
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ApiKey { get; set; }
    }
}
