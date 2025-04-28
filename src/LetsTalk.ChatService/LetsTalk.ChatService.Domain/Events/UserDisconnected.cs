namespace LetsTalk.ChatService.Domain.Events;

public sealed record UserDisconnected(string UserId, string UserName, string? AvatarUrl);
