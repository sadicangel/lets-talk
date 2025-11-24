namespace LetsTalk.Models;

public readonly record struct UserIdentity(string UserId, string UserName, string Email, string? AvatarUrl);
