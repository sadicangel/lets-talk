namespace LetsTalk.Events;

public sealed record class NotificationBroadcast(string ContentType, byte[] Content, DateTimeOffset Timestamp)
    : EventBase(Guid.NewGuid().ToString(), typeof(NotificationBroadcast).Name, DateTimeOffset.UtcNow)
{
    public NotificationBroadcast(Notification notification) : this(notification.ContentType, notification.Content, notification.Timestamp) { }
}
