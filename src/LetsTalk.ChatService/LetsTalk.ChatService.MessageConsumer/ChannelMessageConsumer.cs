using LetsTalk.ChatService.Domain;
using LetsTalk.ChatService.Domain.Entities;
using LetsTalk.Shared.Events;
using MassTransit;

namespace LetsTalk.ChatService.MessageConsumer;

internal class ChannelMessageConsumer(ChatDbContext dbContext) : IConsumer<Message>
{
    public async Task Consume(ConsumeContext<Message> context)
    {
        dbContext.Messages.Add(new ChannelMessage
        {
            ChannelId = context.Message.Channel.ChannelId,
            AuthorId = context.Message.Author.UserId,
            Timestamp = context.Message.Timestamp,
            ContentType = context.Message.ContentType,
            Content = context.Message.Content,
        });

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
