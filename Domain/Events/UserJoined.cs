namespace LetsTalk.Events;

public sealed record class UserJoined(
    string UserId,
    string UserName,
    string UserAvatar,
    string ChannelId,
    string ChannelName,
    string ChannelIcon)
    : EventBase(Guid.NewGuid().ToString(), typeof(UserJoined).Name, DateTimeOffset.UtcNow)
{
    public UserJoined(User user, Channel channel) : this(
        user.Id,
        user.UserName,
        user.Avatar,
        channel.Id,
        channel.DisplayName,
        channel.Icon)
    {
    }
}
