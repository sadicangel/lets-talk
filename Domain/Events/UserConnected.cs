namespace LetsTalk.Events;

public sealed record class UserConnected(string UserId, string UserName, string UserAvatar)
    : EventBase(Guid.NewGuid().ToString(), typeof(UserConnected).Name, DateTimeOffset.UtcNow)
{
    public UserConnected(User user) : this(user.Id, user.UserName, user.Avatar) { }
}
