using LetsTalk.Models;

namespace LetsTalk.Events;

public sealed record class UserConnected(UserIdentity ConnectingUser, IEnumerable<UserIdentity> OnlineUsers) : IHubEvent
{
    public string EventId { get; } = Guid.CreateVersion7().ToString();
    public string EventType => nameof(UserConnected);
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
