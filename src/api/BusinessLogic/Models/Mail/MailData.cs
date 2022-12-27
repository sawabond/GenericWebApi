namespace BusinessLogic.Models.Mail;

public sealed class MailData
{
    public IEnumerable<string> To { get; }

    public IEnumerable<string> Bcc { get; }

    public IEnumerable<string> Cc { get; }

    public string From { get; }

    public string DisplayName { get; }

    public string ReplyTo { get; }

    public string ReplyToName { get; }

    public string Subject { get; }

    public string Body { get; }

    public MailData(IEnumerable<string> to,
                    string subject,
                    string body = null,
                    string from = null,
                    string displayName = null,
                    string replyTo = null,
                    string replyToName = null,
                    IEnumerable<string> bcc = null,
                    IEnumerable<string> cc = null)
    {
        To = to;
        Bcc = bcc ?? new List<string>();
        Cc = cc ?? new List<string>();

        From = from;
        DisplayName = displayName;
        ReplyTo = replyTo;
        ReplyToName = replyToName;

        Subject = subject;
        Body = body;
    }
}
