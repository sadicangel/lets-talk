namespace LetsTalk.Domain.Events;

public sealed record class UserDisconnectedEvent(
    UserDto DisconnectingUser,
    IReadOnlyCollection<UserDto> Users)
    : EventBase;
