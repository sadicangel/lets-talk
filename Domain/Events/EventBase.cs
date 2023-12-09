namespace LetsTalk.Events;

public abstract record class EventBase(string Id, string Type, DateTimeOffset Time);