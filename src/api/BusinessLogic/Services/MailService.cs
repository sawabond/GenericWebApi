using BusinessLogic.Abstractions;
using BusinessLogic.Models.Mail;
using BusinessLogic.Options;
using FluentResults;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BusinessLogic.Services;

public sealed class MailService : IMailService
{
    private readonly MailSettingsOptions _settings;
    private readonly ILogger<MailService> _logger;

    public MailService(
        IOptions<MailSettingsOptions> settingsOptions, 
        ILogger<MailService> logger)
    {
        _settings = settingsOptions.Value;
        _logger = logger;
    }

    public async Task<Result> SendAsync(MailData mailData)
    {
        try
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

            foreach (string mailAddress in mailData.To)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            if (!string.IsNullOrEmpty(mailData.ReplyTo))
            {
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));
            }

            if (mailData.Bcc is not null)
            {
                foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
            }

            if (mailData.Cc != null)
            {
                foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
            }

            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;

            if (_settings.UseSSL)
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
            }
            else if (_settings.UseStartTls)
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            }

            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);

            return Result.Ok();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.Fail("Unable to send email");
        }
    }
}
