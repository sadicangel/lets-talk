namespace LetsTalk.Shared.Events;

public sealed record class ChannelMemberLeft(
    ChannelIdentity Channel,
    UserIdentity LeavingUser) : HubEvent;
