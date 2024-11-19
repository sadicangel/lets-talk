using LetsTalk.Domain.Events;

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

    public MessageEvent ToEvent() => new(Id, Timestamp, ChannelId, AuthorId, ContentType, Content);

    public static implicit operator MessageEvent(Message message) => message.ToEvent();
}
