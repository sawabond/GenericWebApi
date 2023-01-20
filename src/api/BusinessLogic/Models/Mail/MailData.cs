namespace BusinessLogic.Models.Mail;

public sealed class MailData
{
    public MailData(string to,
                    string subject,
                    string body = null,
                    string from = null,
                    string displayName = null)
    {
        To = to;
        From = from;
        DisplayName = displayName;
        Subject = subject;
        Body = body;
    }

    public string To { get; }

    public string From { get; }

    public string DisplayName { get; }

    public string Subject { get; }

    public string Body { get; }
}
