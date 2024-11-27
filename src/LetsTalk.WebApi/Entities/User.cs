namespace LetsTalk.WebApi.Entities;

public sealed class User
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? LastSeenAt { get; set; }
    public List<Channel> Channels { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
}
