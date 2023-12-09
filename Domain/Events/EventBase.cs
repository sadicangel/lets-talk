namespace LetsTalk.Events;

public abstract record class EventBase(string EventId, string EventType, DateTimeOffset EventTimestamp);