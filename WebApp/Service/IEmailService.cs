using System.Net.Mail;

namespace WebApp.Service
{
    public interface IEmailService
    {
        public Task SendEmailAsync(MailMessage email);
    }
}
