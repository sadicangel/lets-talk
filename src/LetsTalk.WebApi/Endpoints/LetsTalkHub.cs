using LetsTalk.Domain.Events;
using LetsTalk.Domain.Services;
using LetsTalk.WebApi.Entities;
using LetsTalk.WebApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi.Endpoints;

internal sealed class LetsTalkHub(
    ConnectionManager connectionManager,
    MessageBatchService messageBatchService,
    ILogger<LetsTalkHub> logger)
    : Hub<ILetsTalkClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = connectionManager.AddConnection(Context);

        var userChannels = connectionManager.GetUserChannels(user);
        foreach (var channel in userChannels)
            await Groups.AddToGroupAsync(Context.ConnectionId, channel.Id);

        // If the user connected for the first time, notify all users and channel members about it.
        var userConnections = connectionManager.GetUserConnections(user);
        if (userConnections.Count == 1)
        {
            var connectedUsers = connectionManager.GetConnectedUsers();

            await Clients.All.OnUserConnected(new UserConnectedEvent(
                ConnectingUser: user,
                Users: connectedUsers));

            logger.LogInformation("{UserName}@{UserId} has joined the server.", user.UserName, user.Id);

            foreach (var channel in userChannels)
            {
                var group = Clients.GroupExcept(channel.Id, userConnections);

                await group.OnChannelMemberJoined(new ChannelMemberJoinedEvent(
                    Channel: channel,
                    JoiningMember: user,
                    Members: connectionManager.GetChannelMembers(channel)));

                await base.OnConnectedAsync();
            }
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = connectionManager.RemoveConnection(Context);

        var userChannels = connectionManager.GetUserChannels(user);
        foreach (var channel in userChannels)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Id);

        // If the user disconnected from the last connection, notify all users and channel members about it.
        var userConnections = connectionManager.GetUserConnections(user);
        if (userConnections.Count == 0)
        {
            foreach (var channel in userChannels)
            {
                var group = Clients.GroupExcept(channel.Id, userConnections);

                await group.OnChannelMemberLeft(new ChannelMemberLeftEvent(
                    Channel: channel,
                    LeavingMember: user,
                    Members: connectionManager.GetChannelMembers(channel)));
            }

            await Clients.All.OnUserDisconnected(new UserDisconnectedEvent(
                DisconnectingUser: user,
                Users: connectionManager.GetConnectedUsers()));

            logger.LogInformation("{Username}@{UserId} has left the server.", user.UserName, user.Id);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string channelId, string contentType, byte[] content)
    {
        var message = new Message
        {
            Id = Guid.CreateVersion7().ToString(),
            Timestamp = DateTimeOffset.UtcNow,
            ChannelId = channelId,
            AuthorId = Context.User.GetUserId(),
            ContentType = contentType,
            Content = content,
        };

        messageBatchService.QueueMessage(message);

        var user = connectionManager.GetUser(Context.User.GetUserId());
        var channel = connectionManager.GetChannel(channelId);

        await Clients.Group(channelId).OnMessage(new MessageEvent
        (
            channel,
            user,
            message.ContentType,
            message.Content
        ));
        logger.LogInformation("{UserName}@{UserId} has sent a message to {ChannelId}.", Context.User.GetUserName(), Context.User.GetUserId(), channelId);
    }
}
