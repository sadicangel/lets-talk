using LetsTalk.Events;
using LetsTalk.Services;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk;

public sealed class AppHub(AppDbContext dbContext, HubConnectionManager connectionManager) : Hub<IAppHubClient>, IAppHubServer
{
    public override async Task OnConnectedAsync()
    {
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        await Task.WhenAll(user.Channels.Select(channel => Groups.AddToGroupAsync(Context.ConnectionId, channel)));

        var userProfile = new UserProfile(user);
        connectionManager.Add(Context.ConnectionId, userProfile);

        await Clients.Others.OnUserConnected(new UserConnected(userProfile));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        await Task.WhenAll(user.Channels.Select(channel => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel)));

        var userProfile = new UserProfile(user);
        connectionManager.Add(Context.ConnectionId, userProfile);

        await Clients.Others.OnUserDisconnected(new UserDisconnected(userProfile));
    }

    public async Task CreateChannel(string channelName, string? channelIcon)
    {
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        var channel = new Channel
        {
            ChannelId = Guid.NewGuid().ToString(),
            ChannelName = channelName,
            ChannelIcon = channelIcon,
            Owner = user.Id,
            Users = { user.Id }
        };
        user.Channels.Add(channel.ChannelId);

        dbContext.Channels.Add(channel);
        await dbContext.SaveChangesAsync();

        await Groups.AddToGroupAsync(Context.ConnectionId, channel.ChannelId);
        await Clients.All.OnChannelCreated(new ChannelCreated(channel));
    }

    public async Task DeleteChannel(string channelId)
    {
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        var channel = await dbContext.Channels.FindAsync(channelId) ?? throw new HubException("Not found");
        if (channel.Owner != user.Id)
            throw new HubException("Unauthorized");

        dbContext.Channels.Remove(channel);
        foreach (var channelUserId in channel.Users)
        {
            var channelUser = await dbContext.Users.FindAsync(channelUserId);
            channelUser?.Channels.Remove(channel.ChannelId);
        }
        await dbContext.SaveChangesAsync();

        await Clients.All.OnChannelDeleted(new ChannelDeleted(channel));
        await Task.WhenAll(channel.Users
            .SelectMany(connectionManager.GetConnectionsByUserId)
            .Select(connection => Groups.RemoveFromGroupAsync(connection, channel.ChannelId)));
    }

    public async Task JoinChannel(string channelId)
    {
        var userProfile = connectionManager.GetUserByConnectionId(Context.ConnectionId);
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        if (!user.Channels.Contains(channelId))
        {
            var channel = await dbContext.Channels.FindAsync(channelId) ?? throw new HubException("Not found");

            user.Channels.Add(channelId);
            channel.Users.Add(user.Id);

            await dbContext.SaveChangesAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
            await Clients.OthersInGroup(channelId).OnUserJoined(new UserJoined(channel, userProfile));
        }
    }

    public async Task LeaveChannel(string channelId)
    {
        var userProfile = connectionManager.GetUserByConnectionId(Context.ConnectionId);
        var user = await dbContext.Users.FindAsync(Context.UserIdentifier!) ?? throw new HubException("Not found");

        if (user.Channels.Contains(channelId))
        {
            var channel = await dbContext.Channels.FindAsync(channelId) ?? throw new HubException("Not found");

            user.Channels.Remove(channelId);
            channel.Users.Remove(user.Id);

            if (channel.Users.Count == 0)
            {
                dbContext.Channels.Remove(channel);
            }
            else if (channel.Owner == user.Id)
            {
                channel.Owner = channel.Users.First();
            }

            await dbContext.SaveChangesAsync();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
            await Clients.OthersInGroup(channelId).OnUserLeft(new UserLeft(channel, userProfile));
        }
    }

    public async Task SendMessage(string channelId, string contentType, byte[] content)
    {
        var userProfile = connectionManager.GetUserByConnectionId(Context.ConnectionId);
        var channel = await dbContext.Channels.FindAsync(channelId) ?? throw new HubException("Not found");
        var message = new Message(contentType, content);

        await Clients.Group(channel.ChannelId).OnChannelMessageBroadcast(new ChannelMessageBroadcast(channel, userProfile, message));
    }
}
