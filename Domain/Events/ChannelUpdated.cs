namespace LetsTalk.Events;

public sealed record class ChannelUpdated(string ChannelId, string ChannelName, string ChannelIcon)
    : EventBase(Guid.NewGuid().ToString(), typeof(ChannelUpdated).Name, DateTimeOffset.UtcNow)
{
    public ChannelUpdated(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
