using System.Net.Mime;
using System.Text;
using LetsTalk.Domain.Services;
using LetsTalk.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi.Services;

[Authorize]
internal sealed class LetsTalkHub(
    ConnectionManager connectionManager,
    LetsTalkDbContext dbContext,
    ILogger<LetsTalkHub> logger)
    : Hub<ILetsTalkClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = connectionManager.AddConnection(Context.ConnectionId, Context.GetUserId());
        var notification = new Notification
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTimeOffset.UtcNow,
            ContentType = MediaTypeNames.Text.Plain,
            Content = Encoding.UTF8.GetBytes($"{user.UserName} has joined the chat."),
        };

        await Clients.All.OnNotificationEvent(notification);
        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("{Username} has joined the chat.", user.UserName);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = connectionManager.RemoveConnection(Context.ConnectionId);
        var notification = new Notification
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTimeOffset.UtcNow,
            ContentType = MediaTypeNames.Text.Plain,
            Content = Encoding.UTF8.GetBytes($"{user.UserName} has left the chat."),
        };

        await Clients.All.OnNotificationEvent(notification);
        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("{Username} has left the chat.", user.UserName);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid channelId, string contentType, byte[] content)
    {
        var message = new Message
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTimeOffset.UtcNow,
            ChannelId = channelId,
            AuthorId = Context.GetUserId(),
            ContentType = contentType,
            Content = content,
        };

        await Clients.All.OnMessageEvent(message);
        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("{UserId} has sent a message.", message.AuthorId);
    }
}
