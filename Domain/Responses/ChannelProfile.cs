namespace LetsTalk.Responses;

public sealed record class ChannelProfile(string ChannelId, string ChannelName, string ChannelIcon)
{
    public ChannelProfile(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
