namespace LetsTalk.Configuration;

public sealed class IdentityApiOptions : ILetsTalkOptions
{
    public const string SectionName = "LetsTalk:IdentityApi";
    static string ILetsTalkOptions.SectionName => SectionName;

    public required string Url { get; init; }
}
