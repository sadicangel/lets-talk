namespace LetsTalk.Services;

public sealed class ChatHubOptions
{
    public const string SectionName = "LetsTalk:ChatHub";
    public required string Url { get; init; }
}
