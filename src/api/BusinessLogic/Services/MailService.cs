using BusinessLogic.Abstractions;
using BusinessLogic.Models.Mail;
using BusinessLogic.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

namespace BusinessLogic.Services;

public sealed class MailService : IMailService
{
    private readonly MailSettingsOptions _settings;
    private readonly ILogger<MailService> _logger;
    private readonly ISendGridClient _sendGridClient;

    public MailService(
        IOptions<MailSettingsOptions> settingsOptions, 
        ILogger<MailService> logger,
        ISendGridClient sendGridClient)
    {
        _settings = settingsOptions.Value;
        _logger = logger;
        _sendGridClient = sendGridClient;
    }

    public async Task<Result> SendAsync(MailData mailData)
    {
        try
        {
            var from = new EmailAddress(_settings.EmailFrom, _settings.NickNameFrom);
            var to = new EmailAddress(mailData.To, mailData.To);
            var subject = mailData.Subject;
            var htmlContent = mailData.Body;

            var message = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

            _logger.LogTrace(
                "Sending email from {from} to {to} with subject {subject}",
                from.Email, to.Email, subject);

            var response = await _sendGridClient.SendEmailAsync(message);

            return response.IsSuccessStatusCode
                ? Result.Ok()
                : Result.Fail(await ExtractErrorFrom(response.Body));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.Fail("Unable to send email");
        }
    }

    private async Task<string> ExtractErrorFrom(HttpContent content)
    {
        var errorResponse = JsonConvert.DeserializeObject<SendGridErrorResponse>(await content.ReadAsStringAsync());
        
        return errorResponse.SendGridErrorMessage;
    }
}
