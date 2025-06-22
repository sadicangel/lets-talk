namespace LetsTalk.Shared.Events;

public sealed record class UserDisconnected(
    UserIdentity DisconnectingUser,
    IEnumerable<UserIdentity> OnlineUsers) : HubEvent;
