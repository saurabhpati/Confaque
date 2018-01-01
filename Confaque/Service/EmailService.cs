using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Confaque.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingOptions _senderOptions;

        public EmailService(IOptions<EmailSettingOptions> emailSenderOptions)
        {
            this._senderOptions = emailSenderOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MimeMessage mimeMessage = new MimeMessage()
            {
                Subject = subject,
                Body = new TextPart("plain")
                {
                    Text = message
                }
            };

            mimeMessage.To.Add(new MailboxAddress(email));
            mimeMessage.From.Add(new MailboxAddress(this._senderOptions.Name, this._senderOptions.UserName));

            using (SmtpClient client = new SmtpClient())
            {
                EmailClientSettingOptions serverOption = this._senderOptions.EmailClientSettings.FirstOrDefault(server => server.IsActive);
                await client.ConnectAsync(serverOption.Server, serverOption.Port).ConfigureAwait(false);
                await client.AuthenticateAsync(this._senderOptions.UserName, this._senderOptions.Password).ConfigureAwait(false);
                await client.SendAsync(mimeMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
