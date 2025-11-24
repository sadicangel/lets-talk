using LetsTalk.Models;

namespace LetsTalk.Events;

public sealed record class Message(ChannelIdentity Channel, UserIdentity Author, string ContentType, byte[] Content) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(Message);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
