namespace LetsTalk;

public sealed class Notification
{
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}