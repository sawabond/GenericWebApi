using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Options.Configuration;

public sealed class MailSettingsSetup : IConfigureOptions<MailSettingsOptions>
{
    private readonly IConfiguration _configuration;

    public MailSettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(MailSettingsOptions options)
    {
        _configuration
            .GetSection(nameof(MailSettingsOptions))
            .Bind(options);
    }
}
