namespace BusinessLogic.Models.Mail;

public sealed class MailData
{
    public MailData(string to, string subject, string body = null)
    {
        To = to;
        Subject = subject;
        Body = body;
    }

    public string To { get; }

    public string Subject { get; }

    public string Body { get; }
}
