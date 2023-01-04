﻿namespace BusinessLogic.Options;

public sealed class MailSettingsOptions
{
    public string DisplayName { get; set; }

    public string From { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public bool UseSSL { get; set; }

    public bool UseStartTls { get; set; }
}
