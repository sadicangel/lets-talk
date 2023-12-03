namespace LetsTalk;

public sealed class Channel
{
    public required string ChannelId { get; init; }
    public required string ChannelName { get; set; }
    public string? ChannelIcon { get; set; }
    public required string Owner { get; set; }
    public HashSet<string> Users { get; init; } = [];
}
