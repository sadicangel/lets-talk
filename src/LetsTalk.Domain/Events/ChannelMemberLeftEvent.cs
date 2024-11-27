namespace LetsTalk.Domain.Events;
public sealed record class ChannelMemberLeftEvent(
    ChannelDto Channel,
    UserDto LeavingMember,
    IReadOnlyCollection<UserDto> Members)
    : EventBase;
