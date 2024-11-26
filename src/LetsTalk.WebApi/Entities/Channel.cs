namespace LetsTalk.WebApi.Entities;

public sealed class Channel
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public required string? IconUrl { get; set; }
    public required string AdminId { get; set; }
    public User Admin { get; set; } = default!;
    public List<User> Members { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
}
