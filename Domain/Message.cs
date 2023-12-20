namespace LetsTalk;

public sealed class Message
{
    public required string Id { get; init; }
    public required Channel Channel { get; init; }
    public required string ChannelId { get; init; }
    public required User Sender { get; init; }
    public required string SenderId { get; init; }
    public required ContentType ContentType { get; init; }
    public required byte[] Content { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}
