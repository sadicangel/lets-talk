namespace LetsTalk.Events;

public sealed record class Notification(string ContentType, byte[] Content) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(Notification);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
