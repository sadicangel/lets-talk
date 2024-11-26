namespace LetsTalk.Domain.Events;

public interface IEvent
{
    string EventId { get; }
    DateTimeOffset Timestamp { get; }
}
