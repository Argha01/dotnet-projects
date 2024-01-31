using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace WebApp.Service
{
    public class EmailService : IEmailService
    {
        //private readonly string sender;

        //private readonly string password;

        //private readonly string host;

        //private readonly int port;

        //public EmailService(string sender,string password, string host, int port)
        //{
        //    this.sender = sender;
        //    this.password = password;
        //    this.host = host;
        //    this.port = port;
        //}

        private readonly IOptions<SmtpSettings> smptpsettings;

        public EmailService(IOptions<SmtpSettings> smptpsettings)
        {
            this.smptpsettings = smptpsettings;
        }
        public async Task SendEmailAsync(MailMessage email)
        {
            var smtpclient = new SmtpClient(smptpsettings.Value.host, smptpsettings.Value.port)
            {
                Credentials = new NetworkCredential(smptpsettings.Value.sender, smptpsettings.Value.password)
            };
            await smtpclient.SendMailAsync(email);
        }
    }
}
