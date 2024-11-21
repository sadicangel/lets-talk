namespace LetsTalk.Domain.Events;
public sealed record class ChannelMemberJoinedEvent(
    string EventId,
    DateTimeOffset Timestamp,
    ChannelDto Channel,
    UserDto JoiningMember,
    IReadOnlyCollection<UserDto> Members)
    : IEvent;
