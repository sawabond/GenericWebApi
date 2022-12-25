namespace BusinessLogic.Options;

public sealed class JwtOptions
{
    public string Key { get; init; }
    public int LifetimeMinutes { get; init; }
}
