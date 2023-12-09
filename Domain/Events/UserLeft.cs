namespace LetsTalk.Events;

public sealed record class UserLeft(
    string UserId,
    string UserName,
    string UserAvatar,
    string ChannelId,
    string ChannelName,
    string ChannelIcon)
    : EventBase(Guid.NewGuid().ToString(), typeof(UserLeft).Name, DateTimeOffset.UtcNow)
{
    public UserLeft(User user, Channel channel) : this(
        user.Id,
        user.UserName,
        user.Avatar,
        channel.Id,
        channel.DisplayName,
        channel.Icon)
    {
    }
}

