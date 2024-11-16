using LetsTalk.Domain.Events;

namespace LetsTalk.WebApi.Entities;

public sealed class Notification : IEntity
{
    public required Guid Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required string ContentType { get; set; }
    public required byte[] Content { get; set; }

    public NotificationEvent ToEvent() => new(Id, Timestamp, ContentType, Content);

    public static implicit operator NotificationEvent(Notification notification) => notification.ToEvent();
}
