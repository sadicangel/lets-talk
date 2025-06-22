namespace LetsTalk.Shared.Events;

public sealed record class ServerNotification(string ContentType, byte[] Content) : HubEvent;
