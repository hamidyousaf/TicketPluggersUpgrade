﻿using Microsoft.Extensions.Configuration;
using TP.Upgrade.Application.Common.Contracts.IServices;

namespace TP.Upgrade.Application.Services
{
    public sealed class SendGridMailService : IMailService
    {
        private IConfiguration _configuration;

        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];
           // var client = new SendGridClient(apiKey);
           // var from = new EmailAddress("test@authdemo.com", "JWT Auth Demo");
           // var to = new EmailAddress(toEmail);
           // var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
           // var response = await client.SendEmailAsync(msg);
        }
    }
}
