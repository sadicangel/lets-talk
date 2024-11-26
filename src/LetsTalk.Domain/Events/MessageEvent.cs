namespace LetsTalk.Domain.Events;

public sealed record class MessageEvent(
    string EventId,
    DateTimeOffset Timestamp,
    ChannelDto Channel,
    UserDto Author,
    string ContentType,
    byte[] Content)
    : IEvent;
