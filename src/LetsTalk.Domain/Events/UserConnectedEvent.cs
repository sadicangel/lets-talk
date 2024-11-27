namespace LetsTalk.Domain.Events;

public sealed record class UserConnectedEvent(
    UserDto ConnectingUser,
    IReadOnlyCollection<UserDto> Users)
    : EventBase;
