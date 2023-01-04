namespace BusinessLogic.Options;

public sealed class GoogleAuthOptions
{
    public const string Section = "Authentication:Google";

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }
}
