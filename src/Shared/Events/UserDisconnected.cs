using LetsTalk.Models;

namespace LetsTalk.Events;

public sealed record class UserDisconnected(UserIdentity DisconnectingUser, IEnumerable<UserIdentity> OnlineUsers) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(UserDisconnected);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
