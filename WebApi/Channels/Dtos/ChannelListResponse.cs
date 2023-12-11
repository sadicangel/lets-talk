namespace LetsTalk.Channels.Dtos;

public sealed record class ChannelListResponse(IReadOnlyList<ChannelListChannel> Channels, string? After);

public sealed record class ChannelListChannel(string ChannelId, string ChannelName, string ChannelIcon)
{
    public ChannelListChannel(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
