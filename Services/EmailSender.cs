using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Security;

namespace ManagementPanel.Services
{
    public class EmailSender
    {
        private readonly EmailSettings _settings;
        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendEmailAsync(string receiverEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(receiverEmail));
            email.Subject = subject;
            email.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, false);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

