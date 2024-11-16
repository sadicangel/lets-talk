namespace LetsTalk.Domain.Events;
public sealed record class UserConnectedEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    string ContentType,
    byte[] Content) : IEvent
{

}
