namespace LetsTalk.Account.Dtos;

public sealed record class UserChannelListResponse(IEnumerable<UserChannelListChannel> Channels)
{
    public UserChannelListResponse(IEnumerable<Channel> channels) : this(channels.Select(x => new UserChannelListChannel(x))) { }
}

public sealed record class UserChannelListChannel(string ChannelId, string ChannelName, string ChannelIcon)
{
    public UserChannelListChannel(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon) { }
}

