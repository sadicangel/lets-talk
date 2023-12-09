namespace LetsTalk.Events;
public sealed record class MessageBroadcast(
    string SenderId,
    string SenderName,
    string SenderAvatar,
    string ChannelId,
    string ChannelName,
    string ChannelIcon,
    string ContentType,
    byte[] Content,
    DateTimeOffset Timestamp)
    : EventBase(Guid.NewGuid().ToString(), typeof(MessageBroadcast).Name, DateTimeOffset.UtcNow)
{
    public MessageBroadcast(Message message) : this(
        message.Sender.Id,
        message.Sender.UserName,
        message.Sender.Avatar,
        message.Channel.Id,
        message.Channel.DisplayName,
        message.Channel.Icon,
        message.ContentType,
        message.Content,
        message.Timestamp)
    { }
}
