namespace LetsTalk.ChatService.Domain.Events;

public sealed record UserConnected(string UserId, string UserName, string? AvatarUrl);
