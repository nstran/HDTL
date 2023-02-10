using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MimeKit.Text;

namespace TN.TNM.DataAccess.Helper
{
    public static class EMailkit
    {
        public static async Task SendAsync()
        {
            var secrets = new ClientSecrets
            {
                ClientId = "823817780930-6tamlq6bp93tt0um00ettugf4dt7ktgj.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                DataStore = new FileDataStore(AppDomain.CurrentDomain.BaseDirectory),
                Scopes = new[] { GmailService.Scope.MailGoogleCom },
                ClientSecrets = secrets
            });

            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);
            var credential = await authCode.AuthorizeAsync("truonggiangdev1992@gmail.com", CancellationToken.None);

            if (authCode.ShouldRequestAuthorizationCode(credential.Token))
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
            }

            var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.None);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(oauth2);

                var mailMessage = new MimeMessage();
                mailMessage.From.Add(MailboxAddress.Parse("truonggiangdev1992@gmail.com"));
                mailMessage.To.Add(MailboxAddress.Parse("anhgiangtg@gmail.com"));
                mailMessage.Subject = "Test Email Subject";
                mailMessage.Body = new TextPart(TextFormat.Html) { Text = "<h1>Example HTML Message Body</h1>" };

                client.Send(mailMessage);
                client.Disconnect(true);
            }
        }
    }
}
