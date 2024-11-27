namespace LetsTalk.Domain.Events;

public sealed record class NotificationEvent(
    string ContentType,
    byte[] Content)
    : EventBase;
