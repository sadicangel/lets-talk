using LetsTalk.Models;

namespace LetsTalk.Events;

public sealed record class ChannelMemberLeft(ChannelIdentity Channel, UserIdentity LeavingUser) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(ChannelMemberLeft);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
