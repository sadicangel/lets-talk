namespace LetsTalk;

public sealed record class Message(string ContentType, byte[] Content)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}