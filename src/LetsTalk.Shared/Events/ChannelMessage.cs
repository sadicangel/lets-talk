namespace LetsTalk.Shared.Events;
public sealed record class ChannelMessage(
    string ChannelId,
    UserIdentity Author,
    string ContentType,
    byte[] Content) : HubEvent;
