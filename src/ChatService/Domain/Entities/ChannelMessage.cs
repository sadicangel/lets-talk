namespace LetsTalk.Chat.Entities;

public sealed class ChannelMessage
{
    public string Id { get; init; } = null!;
    public uint Version { get; init; }
    public required string ChannelId { get; init; }
    public Channel Channel { get; init; } = null!;
    public required string AuthorId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}
