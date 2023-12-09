namespace LetsTalk.Events;

public sealed record class UserDisconnected(string UserId, string UserName, string UserAvatar)
    : EventBase(Guid.NewGuid().ToString(), typeof(UserDisconnected).Name, DateTimeOffset.UtcNow)
{
    public UserDisconnected(User user) : this(user.Id, user.UserName, user.Avatar) { }
}
