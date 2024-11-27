namespace LetsTalk.Domain.Events;

public abstract record class EventBase(
    string EventId,
    string EventName,
    DateTimeOffset Timestamp)
{
    protected EventBase() : this(
        EventId: Guid.NewGuid().ToString(),
        EventName: "",
        Timestamp: DateTimeOffset.UtcNow)
    {
        EventName = GetType().Name;
    }
}
