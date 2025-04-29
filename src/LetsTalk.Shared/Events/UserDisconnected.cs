namespace LetsTalk.Shared.Events;

public sealed record class UserDisconnected(UserIdentity DisconnectingUser, IEnumerable<UserIdentity> Users) : HubEvent;
