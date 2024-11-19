namespace LetsTalk.WebApi.Entities;

public sealed class Message
{
    public required Guid Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required Guid ChannelId { get; set; }
    public Channel Channel { get; set; } = default!;
    public required Guid AuthorId { get; set; }
    public User Author { get; set; } = default!;
    public required string ContentType { get; set; }
    public required byte[] Content { get; set; }
}
