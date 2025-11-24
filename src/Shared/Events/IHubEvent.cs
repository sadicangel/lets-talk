namespace LetsTalk.Events;

public interface IHubEvent
{
    string EventId { get; }
    string EventType { get; }
    DateTimeOffset Timestamp { get; }
}
