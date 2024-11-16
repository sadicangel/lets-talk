namespace LetsTalk.Domain.Events;

public sealed record class MessageEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    Guid ChannelId,
    Guid SenderId,
    string ContentType,
    byte[] Content)
    : IEvent;
