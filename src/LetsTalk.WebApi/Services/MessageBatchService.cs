using System.Threading.Channels;
using LetsTalk.WebApi.Entities;
using Channel = System.Threading.Channels.Channel;

namespace LetsTalk.WebApi.Services;

public sealed class MessageBatchService(IServiceScopeFactory serviceScopeFactory, ILogger<MessageBatchService> logger) : BackgroundService
{
    private readonly Channel<Message> _messageChannel = Channel.CreateUnbounded<Message>();

    public void QueueMessage(Message message) => _messageChannel.Writer.TryWrite(message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var messages = new List<Message>();

        while (!stoppingToken.IsCancellationRequested)
        {
            while (await _messageChannel.Reader.WaitToReadAsync(stoppingToken))
            {
                while (_messageChannel.Reader.TryRead(out var message))
                {
                    messages.Add(message);

                    if (messages.Count >= 100) // Batch size
                    {
                        await SaveMessagesAsync(messages);
                        messages.Clear();
                    }
                }
            }

            if (messages.Count > 0)
            {
                await SaveMessagesAsync(messages);
                messages.Clear();
            }

            await Task.Delay(1000, stoppingToken); // Batch interval
        }
    }

    private async Task SaveMessagesAsync(List<Message> messages)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LetsTalkDbContext>();

        dbContext.Messages.AddRange(messages);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Saved {Count} messages to the database.", messages.Count);
    }
}
