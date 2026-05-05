namespace LetsTalk.Configuration;

public sealed class LetsTalkJwtOptions : ILetsTalkOptions
{
    public const string SectionName = "LetsTalk:Jwt";
    static string ILetsTalkOptions.SectionName => SectionName;

    public required string SecurityKey { get; init; }
    public string Issuer { get; init; } = "letstalk-identity";
    public string Audience { get; init; } = "letstalk";
    public long TokenExpirationSeconds { get; init; } = 3600;
    public int RefreshTokenSize { get; init; } = 32;
}
