using LetsTalk.Domain.Events;

namespace LetsTalk.WebApi.Entities;

public sealed class Message : IEntity
{
    public required Guid Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required Guid ChannelId { get; set; }
    public Channel Channel { get; set; } = default!;
    public required Guid SenderId { get; set; }
    public User Sender { get; set; } = default!;
    public required string ContentType { get; set; }
    public required byte[] Content { get; set; }

    public MessageEvent ToEvent() => new(Id, Timestamp, ChannelId, SenderId, ContentType, Content);

    public static implicit operator MessageEvent(Message message) => message.ToEvent();
}
