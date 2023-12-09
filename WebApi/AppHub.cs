using LetsTalk.Events;
using LetsTalk.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk;

public sealed class AppHub(AppDbContext dbContext, HubConnectionManager connectionManager) : Hub<IAppHubClient>, IAppHubServer
{
    public override async Task OnConnectedAsync()
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .AsNoTracking()
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        await Task.WhenAll(user.Channels.Select(channel => Groups.AddToGroupAsync(Context.ConnectionId, channel.Id)));

        connectionManager.Add(Context.ConnectionId, user.Id);

        await Clients.Others.OnUserConnected(new UserConnected(user));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .AsNoTracking()
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        await Task.WhenAll(user.Channels.Select(channel => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Id)));

        connectionManager.Remove(Context.ConnectionId);

        await Clients.Others.OnUserDisconnected(new UserDisconnected(user));
    }

    public async Task CreateChannel(string channelName, string? channelIcon)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        var channelId = Guid.NewGuid().ToString();
        var channel = new Channel
        {
            Id = channelId,
            DisplayName = channelName,
            Icon = channelIcon ?? $"https://api.dicebear.com/7.x/shapes/svg?seed={channelId}",
            Admin = user,
            Participants = [user]
        };
        user.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        await Groups.AddToGroupAsync(Context.ConnectionId, channel.Id);
        await Clients.All.OnChannelCreated(new ChannelCreated(channel));
    }

    public async Task DeleteChannel(string channelId)
    {
        var channel = await dbContext.Channels
            .Include(x => x.Admin)
            .Include(x => x.Participants)
            .SingleAsync(x => x.Id == channelId);

        if (channel.Admin.Id != Context.UserIdentifier)
            throw new HubException("Unauthorized");

        dbContext.Channels.Remove(channel);

        await dbContext.SaveChangesAsync();

        await Clients.All.OnChannelDeleted(new ChannelDeleted(channel));
        await Task.WhenAll(channel.Participants
            .SelectMany(u => connectionManager.GetConnectionsByUserId(u.Id))
            .Select(connection => Groups.RemoveFromGroupAsync(connection, channel.Id)));
    }

    public async Task JoinChannel(string channelId)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        if (!user.Channels.Exists(c => c.Id == channelId))
        {
            var channel = await dbContext.Channels
                .Include(x => x.Participants)
                .SingleAsync(x => x.Id == channelId);

            channel.Participants.Add(user);

            await dbContext.SaveChangesAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
            await Clients.OthersInGroup(channelId).OnUserJoined(new UserJoined(user, channel));
        }
    }

    public async Task LeaveChannel(string channelId)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        if (user.Channels.Exists(c => c.Id == channelId))
        {
            var channel = await dbContext.Channels
                .Include(x => x.Admin)
                .Include(x => x.Participants)
                .SingleAsync(x => x.Id == channelId);

            channel.Participants.Remove(user);

            if (channel.Participants.Count == 0)
            {
                dbContext.Channels.Remove(channel);
            }
            else if (channel.Admin.Id == user.Id)
            {
                // TODO: Pick oldest member instead or disallow while admin.
                channel.Admin = channel.Participants[0];
            }

            await dbContext.SaveChangesAsync();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
            await Clients.OthersInGroup(channelId).OnUserLeft(new UserLeft(user, channel));
        }
    }

    public async Task SendMessage(string channelId, string contentType, byte[] content)
    {
        var user = await dbContext.Users
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        var channel = await dbContext.Channels
                .SingleAsync(x => x.Id == channelId);

        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            Channel = channel,
            Sender = user,
            ContentType = contentType,
            Content = content,
        };

        channel.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        await Clients.Group(channel.Id).OnMessageBroadcast(new MessageBroadcast(message));
    }
}
