namespace LetsTalk.Events;

public sealed record class NotificationBroadcast(string Content, DateTimeOffset Timestamp)
    : EventBase(Guid.NewGuid().ToString(), typeof(NotificationBroadcast).Name, DateTimeOffset.UtcNow)
{
    public NotificationBroadcast(Notification notification) : this(notification.Content, notification.Timestamp) { }
}
