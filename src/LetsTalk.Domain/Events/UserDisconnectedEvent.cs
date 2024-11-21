namespace LetsTalk.Domain.Events;

public sealed record class UserDisconnectedEvent(
    string EventId,
    DateTimeOffset Timestamp,
    UserDto DisconnectingUser,
    IReadOnlyCollection<UserDto> Users);
