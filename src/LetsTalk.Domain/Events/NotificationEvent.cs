﻿namespace LetsTalk.Domain.Events;

public sealed record class NotificationEvent(
    Guid EventId,
    DateTimeOffset Timestamp,
    string ContentType,
    byte[] Content)
    : IEvent;
