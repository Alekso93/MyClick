using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain;
using EShop.Domain.DomainModels;
using EShop.Service.Interface;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Tls;

namespace EShop.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task SendEmailAsync(List<EmailMessage> allMails)
        {
            List<MimeMessage> messages = new List<MimeMessage>();
            foreach (var item in allMails)
            {
                var emailMessage = new MimeMessage
                {
                    Sender = new MailboxAddress(_emailSettings.SenderName, _emailSettings.SmtpUserName),
                    Subject = item.Subject
                };

                emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SmtpUserName));
                emailMessage.Body = new TextPart(TextFormat.Plain)
                {
                    Text = item.Content
                };
                emailMessage.To.Add(MailboxAddress.Parse(item.MailTo));
                messages.Add(emailMessage);
            }
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOption = _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpServerPort, socketOption);
                    if (!string.IsNullOrEmpty(_emailSettings.SmtpUserName))
                    {
                        await client.AuthenticateAsync(_emailSettings.SmtpUserName, _emailSettings.SmtpPassword);
                    }
                    foreach (var message in messages)
                    {
                        await client.SendAsync(message);
                    }
                    await client.DisconnectAsync(true);
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }

    }
}
