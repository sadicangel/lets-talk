namespace LetsTalk.Events;

public sealed record class ChannelDeleted(string ChannelId, string ChannelName, string ChannelIcon)
    : EventBase(Guid.NewGuid().ToString(), typeof(ChannelDeleted).Name, DateTimeOffset.UtcNow)
{
    public ChannelDeleted(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
