using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendConfirmationEmailAsync(string to, string confirmationLink);
        Task SendPasswordResetEmailAsync(string to, string resetLink);
    }
}
