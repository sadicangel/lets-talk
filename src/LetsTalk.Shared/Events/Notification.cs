namespace LetsTalk.Shared.Events;

public sealed record class Notification(string ContentType, byte[] Content) : HubEvent;
