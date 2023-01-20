using Azure;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.Mail;
using BusinessLogic.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

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
            var client = new SendGridClient(_settings.SendGridKey);

            var from = new EmailAddress(mailData.From, mailData.DisplayName);
            var to = new EmailAddress(mailData.To, mailData.To);
            var subject = mailData.Subject;
            var htmlContent = mailData.Body;

            var message = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

            _logger.LogTrace(
                "Sending email from {from} to {to} with subject {subject}",
                from.Email, to.Email, subject);

            var response = await client.SendEmailAsync(message);

            return response.IsSuccessStatusCode
                ? Result.Ok()
                : Result.Fail(await response.Body.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.Fail("Unable to send email");
        }
    }

    //private async Task<string[]> ExtractErrorsFrom(HttpContent content)
    //{
    //    var response = JsonConvert.DeserializeObject<>(await content.ReadAsStringAsync());
    //}

    //private class
}
