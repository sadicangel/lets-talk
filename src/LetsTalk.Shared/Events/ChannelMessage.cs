namespace LetsTalk.Shared.Events;
public sealed record class ChannelMessage(
    ChannelIdentity Channel,
    UserIdentity Author,
    string ContentType,
    byte[] Content) : HubEvent;
