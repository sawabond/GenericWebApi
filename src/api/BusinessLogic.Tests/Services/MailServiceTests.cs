using BusinessLogic.Models.Mail;
using BusinessLogic.Options;
using BusinessLogic.Services;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;

namespace BusinessLogic.Tests.Services;

public class MailServiceTests
{
	private readonly MailService _mailService;
	private readonly MailSettingsOptions _options;
	private readonly Mock<ILogger<MailService>> _logger;
	private readonly Mock<ISendGridClient> _sendGridClient;

	public MailServiceTests()
	{
		_logger= new Mock<ILogger<MailService>>();
		_options = new MailSettingsOptions
		{
			EmailFrom = "from@mail.com",
			NickNameFrom = "display_name",
			SendGridKey = "some_api_key"
		};
		_sendGridClient = new Mock<ISendGridClient>();

        _sendGridClient
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(new Response(
                HttpStatusCode.OK,
				JsonContent.Create(new { }),
                default));

        var options = new Mock<IOptions<MailSettingsOptions>>();
		options.SetupGet(x => x.Value).Returns(_options);

		_mailService = new MailService(options.Object, _logger.Object, _sendGridClient.Object);
	}

	private MailData ValidMailData =>
		new MailData("to@mail.com", "subject", "HTML body info");

	[Fact]
	public async void SendAsync_ReturnsFail_IfEmailNotSent()
	{
		_sendGridClient
			.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
			.ReturnsAsync(new Response(
				HttpStatusCode.BadRequest, 
				JsonContent.Create(new { SendGridErrorMessage = "Api error" }),
				default));

		var result = await _mailService.SendAsync(ValidMailData);

		result.IsSuccess.Should().BeFalse();
		result.Errors.Should().ContainEquivalentOf(new Error("Api error"));
    }

	[Fact]
	public async void SendAsync_CallsSendEmailAsync_WithCorrectParameters()
	{
        await _mailService.SendAsync(ValidMailData);

		Expression<Func<SendGridMessage, bool>> expectedMessage =
			msg => msg.From.Email == _options.EmailFrom
			&& msg.Contents.Any(c => c.Value == ValidMailData.Body)
			&& msg.Personalizations.Any(p => p.Subject == ValidMailData.Subject)
			&& msg.Personalizations.Any(
				p => p.Tos.Any(to => to.Email == ValidMailData.To));

        _sendGridClient.Verify(x => x.SendEmailAsync(It.Is(expectedMessage), default), Times.Once);
    }

	[Fact]
	public async void SendAsync_CatchesException_IfSendEmailAsyncThrows()
	{
        _sendGridClient
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
			.ThrowsAsync(new Exception());

		var result = await _mailService.SendAsync(ValidMailData);

		result.IsSuccess.Should().BeFalse();
		result.Errors.Should().ContainEquivalentOf(new Error("Unable to send email"));
    }
}
