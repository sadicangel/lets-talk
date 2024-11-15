namespace LetsTalk.Domain.Events;

public interface IEvent
{
    Guid EventId { get; }
    DateTimeOffset Timestamp { get; }
}
