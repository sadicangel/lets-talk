namespace LetsTalk;

public sealed class Channel
{
    public required string Id { get; init; }
    public required string DisplayName { get; set; }
    public string Icon { get; set; } = string.Empty;
    public required User Admin { get; set; }
    public required List<User> Participants { get; init; }
    public List<Message> Messages { get; init; } = [];
}
