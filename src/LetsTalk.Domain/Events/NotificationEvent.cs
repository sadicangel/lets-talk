namespace LetsTalk.Domain.Events;

public sealed record class NotificationEvent(
    string EventId,
    DateTimeOffset Timestamp,
    string ContentType,
    byte[] Content)
    : IEvent;
