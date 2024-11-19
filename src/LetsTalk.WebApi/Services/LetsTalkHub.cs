using LetsTalk.Domain.Events;
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

        await Clients.All.OnUserConnected(new UserConnectedEvent(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            user.Id,
            user.UserName,
            user.AvatarUrl));

        logger.LogInformation("{UserName}@{UserId} has joined the chat.", user.UserName, user.Id);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = connectionManager.RemoveConnection(Context.ConnectionId);

        await Clients.All.OnUserDisconnected(new UserDisconnectedEvent(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            user.Id,
            user.UserName,
            user.AvatarUrl));

        logger.LogInformation("{Username}@{UserId} has left the chat.", user.UserName, user.Id);

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

        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();

        await Clients.All.OnMessage(new MessageEvent
        (
            message.Id,
            message.Timestamp,
            message.ChannelId,
            Context.GetUserId(),
            Context.GetUserName(),
            Context.GetUserAvatarUri(),
            message.ContentType,
            message.Content
        ));
        logger.LogInformation("{UserName}@{UserId} has sent a message.", Context.GetUserName(), Context.GetUserId());
    }
}
