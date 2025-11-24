using LetsTalk.Models;

namespace LetsTalk.Events;

public sealed record class ChannelMemberJoined(ChannelIdentity Channel, UserIdentity JoiningUser) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(ChannelMemberJoined);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
