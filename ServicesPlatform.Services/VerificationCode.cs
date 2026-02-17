using Microsoft.Extensions.Configuration;
using ServicesPlatform.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Services
{
    public class VerificationCode : IVerificationCode
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _password;
        public VerificationCode(IConfiguration configuration)
        {
            _smtpHost = configuration["EmailSettings:SMTPHost"];
            _smtpPort = int.Parse(configuration["EmailSettings:SMTPPort"]);
            _senderEmail = configuration["EmailSettings:SenderEmail"];
            _senderName = configuration["EmailSettings:SenderName"];
            _password = configuration["EmailSettings:Password"];
        }
        public async Task<bool> SendVerificationCode(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_senderEmail, _password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, _senderName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    await client.SendMailAsync(mailMessage);

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
