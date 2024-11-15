namespace LetsTalk.Domain.Events;

public sealed record class MessageEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    Guid ChannelId,
    Guid SenderId,
    string ContentType,
    byte[] Content)
    : IEvent
{
    public MessageEvent(Guid channelId, Guid senderId, string contentType, byte[] content)
        : this(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            channelId,
            senderId,
            contentType,
            content)
    {
    }
}
