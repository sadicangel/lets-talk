﻿namespace LetsTalk.Domain.Events;

public sealed record class UserConnectedEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    Guid UserId,
    string UserName,
    string? UserAvatarUrl);
