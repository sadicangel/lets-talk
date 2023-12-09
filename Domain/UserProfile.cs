namespace LetsTalk;

public sealed record class UserProfile(string UserId, string UserName, string UserAvatar, IEnumerable<ChannelProfile> Channels)
{
    public UserProfile(User user) : this(user.Id, user.UserName, user.Avatar, user.Channels.Select(c => new ChannelProfile(c))) { }
}

public sealed record class ChannelProfile(string ChannelId, string ChannelName, string ChannelIcon)
{
    public ChannelProfile(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
