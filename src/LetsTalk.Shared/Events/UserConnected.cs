namespace LetsTalk.Shared.Events;

public sealed record class UserConnected(UserIdentity ConnectingUser, IEnumerable<UserIdentity> Users) : HubEvent;
