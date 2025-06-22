using LetsTalk.Shared;
using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.ChatService.WebApi.Services;

internal sealed class ChatHub(
    ConnectionManager connectionManager,
    IChannelService channelService, ILogger<ChatHub> logger)
    : Hub<ILetsTalkClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User.GetUserIdentity();

        // User connected
        connectionManager.AddConnection(user, Context.ConnectionId);
        logger.LogInformation("User connected: {@User} ({ConnectionId})", user, Context.ConnectionId);

        // Add user to groups based on channels
        var response = await channelService.GetChannels(user.UserId);
        await Task.WhenAll(response.Channels.Select(channel => Groups.AddToGroupAsync(Context.ConnectionId, channel)));

        // Notify users in the same groups about user joining (for the first time)
        var connections = connectionManager.GetConnections(user.UserId);
        if (connections.Count == 1)
        {
            await Clients.All.OnUserConnected(new UserConnected(user, connectionManager.GetOnlineUsers()));
            logger.LogInformation("User has joined the server: {@User} ({ConnectionId})", user, Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User.GetUserIdentity();

        // User disconnected
        connectionManager.RemoveConnection(Context.ConnectionId);
        logger.LogInformation("User disconnected: {@User} ({ConnectionId})", user, Context.ConnectionId);

        // Remove user from groups
        var response = await channelService.GetChannels(user.UserId);
        await Task.WhenAll(response.Channels.Select(channel => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel)));

        // Notify users in the same groups about user leaving
        var connections = connectionManager.GetConnections(user.UserId);
        if (connections.Count == 0)
        {
            await Clients.All.OnUserDisconnected(new UserDisconnected(user, connectionManager.GetOnlineUsers()));
            logger.LogInformation("User has left the server: {@User} ({ConnectionId})", user, Context.ConnectionId);
        }
    }

    public async Task SendChannelMessage(string channelId, string contentType, byte[] content)
    {
        var message = new ChannelMessage(channelId, Context.User.GetUserIdentity(), contentType, content);

        // Send message to all clients in the group.
        await Clients.Group(channelId).OnMessage(message);

        logger.LogInformation("User {UserId} sent message to channel {ChannelId}: {@Message}", Context.User.GetUserId(), channelId, message);

        // Send message to queue so that message can be persisted in the database.
    }
}
