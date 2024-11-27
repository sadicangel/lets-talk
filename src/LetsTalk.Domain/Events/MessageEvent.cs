namespace LetsTalk.Domain.Events;

public sealed record class MessageEvent(
    ChannelDto Channel,
    UserDto Author,
    string ContentType,
    byte[] Content)
    : EventBase;
