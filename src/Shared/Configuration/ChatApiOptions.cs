namespace LetsTalk.Configuration;

public sealed class ChatApiOptions : ILetsTalkOptions
{
    public const string SectionName = "LetsTalk:ChatApi";
    static string ILetsTalkOptions.SectionName => SectionName;

    public required string Url { get; init; }

    public string HubName { get; init; } = "hub";
}
