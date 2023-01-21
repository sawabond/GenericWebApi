namespace BusinessLogic.Options;

public sealed class MailSettingsOptions
{
    public string SendGridKey { get; set; }

    public string EmailFrom { get; set; }

    public string NickNameFrom { get; set; }
}
