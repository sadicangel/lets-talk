namespace LetsTalk.Domain.Events;

public sealed record class UserDisconnectedEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    Guid UserId,
    string UserName,
    string? UserAvatarUrl);
