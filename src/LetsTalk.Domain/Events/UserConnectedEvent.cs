namespace LetsTalk.Domain.Events;

public sealed record class UserConnectedEvent(
    string EventId,
    DateTimeOffset Timestamp,
    UserDto ConnectingUser,
    IReadOnlyCollection<UserDto> Users);
