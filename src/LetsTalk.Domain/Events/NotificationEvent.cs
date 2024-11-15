using System.Text;

namespace LetsTalk.Domain.Events;

public sealed record class NotificationEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    string ContentType,
    byte[] Content)
    : IEvent
{
    public NotificationEvent(string content)
        : this(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "text/plain",
            Encoding.UTF8.GetBytes(content))
    {
    }
}
