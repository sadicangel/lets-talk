namespace LetsTalk.Shared.Events;

public sealed record class Message(
    ChannelIdentity Channel,
    UserIdentity Author,
    string ContentType,
    byte[] Content) : HubEvent;
