namespace LetsTalk.Shared.Events;
public sealed record class ChannelMessage(string ChannelId, string UserId, string ContentType, byte[] Content) : HubEvent;
