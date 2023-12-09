

namespace LetsTalk.Events;

public sealed record class ChannelCreated(string ChannelId, string ChannelName, string ChannelIcon)
    : EventBase(Guid.NewGuid().ToString(), typeof(ChannelCreated).Name, DateTimeOffset.UtcNow)
{
    public ChannelCreated(Channel channel) : this(channel.Id, channel.DisplayName, channel.Icon)
    {
    }
}
