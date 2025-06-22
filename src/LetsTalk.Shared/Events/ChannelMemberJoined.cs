namespace LetsTalk.Shared.Events;

public sealed record class ChannelMemberJoined(
    ChannelIdentity Channel,
    UserIdentity JoiningUser) : HubEvent;
