using System.Security.Claims;
using LetsTalk.ChatService.Domain;
using LetsTalk.ChatService.Domain.Events;
using LetsTalk.ChatService.WebApi.ChannelService;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.ChatService.WebApi.Services;

internal sealed class ChatHub(
    ConnectionManager connectionManager,
    IChannelService channelService, ILogger<ChatHub> logger)
    : Hub<ILetsTalkClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User.GetUserInfo();

        // User connected
        connectionManager.AddConnection(user.UserId, Context.ConnectionId);
        logger.LogInformation("User connected: {@User}", user);

        // Add user to groups based on channels
        var response = await channelService.GetUserChannelListAsync(user.UserId);
        await Task.WhenAll(response.Channels.Select(channel => Groups.AddToGroupAsync(Context.ConnectionId, channel.Id)));

        // Notify users in the same groups about user joining
        await Clients.Groups(response.Channels.Select(channel => channel.Id))
            .OnUserConnected(new UserConnected(user.UserId, user.UserName, user.AvatarUrl));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User.GetUserInfo();

        // User disconnected
        connectionManager.RemoveConnection(Context.ConnectionId);
        logger.LogInformation("User disconnected: {@User}", user);

        // Remove user from groups
        var response = await channelService.GetUserChannelListAsync(user.UserId);
        await Task.WhenAll(response.Channels.Select(channel => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Id)));

        // Notify users in the same groups about user leaving
        await Clients.Groups(response.Channels.Select(channel => channel.Id))
            .OnUserDisconnected(new UserDisconnected(user.UserId, user.UserName, user.AvatarUrl));
    }
}
