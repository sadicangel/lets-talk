namespace LetsTalk.Services;

public sealed class IdentityApiOptions
{
    public const string SectionName = "LetsTalk:IdentityApi";

    public required string Url { get; init; }
}
