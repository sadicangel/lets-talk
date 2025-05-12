namespace LetsTalk.Shared.Events;
public abstract record class HubEvent(string EventId, string EventName, DateTimeOffset Timestamp)
{
    protected HubEvent() : this(default!, default!, default!)
    {
        var timestamp = DateTimeOffset.UtcNow;

        EventId = Guid.CreateVersion7(timestamp).ToString();
        EventName = GetType().Name;
        Timestamp = timestamp;
    }
}
