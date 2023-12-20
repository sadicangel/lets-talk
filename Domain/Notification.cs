namespace LetsTalk;

public sealed class Notification
{
    public required string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}