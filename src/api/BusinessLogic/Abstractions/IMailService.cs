using BusinessLogic.Models.Mail;
using FluentResults;

namespace BusinessLogic.Abstractions;

public interface IMailService
{
    Task<Result> SendAsync(MailData mailData);
}
