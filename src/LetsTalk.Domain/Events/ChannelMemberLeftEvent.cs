namespace LetsTalk.Domain.Events;
public sealed record class ChannelMemberLeftEvent(
    string EventId,
    DateTimeOffset Timestamp,
    ChannelDto Channel,
    UserDto LeavingMember,
    IReadOnlyCollection<UserDto> Members)
    : IEvent;
