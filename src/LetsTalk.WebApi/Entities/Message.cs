namespace LetsTalk.WebApi.Entities;

public sealed class Message
{
    public required string Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required string ChannelId { get; set; }
    public Channel Channel { get; set; } = default!;
    public required string AuthorId { get; set; }
    public User Author { get; set; } = default!;
    public required string ContentType { get; set; }
    public required byte[] Content { get; set; }
}
