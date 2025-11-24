namespace LetsTalk.Services;

public sealed class ChatApiOptions
{
    public const string SectionName = "LetsTalk:ChatApi";
    public required string Url { get; init; }
}
