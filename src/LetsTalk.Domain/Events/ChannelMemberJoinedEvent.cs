namespace LetsTalk.Domain.Events;
public sealed record class ChannelMemberJoinedEvent(
    ChannelDto Channel,
    UserDto JoiningMember,
    IReadOnlyCollection<UserDto> Members)
    : EventBase;
