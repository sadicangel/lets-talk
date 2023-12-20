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

        connectionManager.Add(user.Id, Context.ConnectionId);

        await Clients.Others.OnUserConnected(new UserConnected(user));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .AsNoTracking()
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        await Task.WhenAll(user.Channels.Select(channel => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Id)));

        connectionManager.Remove(user.Id);

        await Clients.Others.OnUserDisconnected(new UserDisconnected(user));
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

    public async Task SendMessage(string channelId, ContentType contentType, byte[] content)
    {
        var user = await dbContext.Users
            .SingleAsync(x => x.Id == Context.UserIdentifier!);

        var channel = await dbContext.Channels
                .SingleAsync(x => x.Id == channelId);

        var message = new Message
        {
            Id = Uuid.Create(),
            Channel = channel,
            ChannelId = channel.Id,
            Sender = user,
            SenderId = user.Id,
            ContentType = contentType,
            Content = content,
        };

        channel.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        await Clients.Group(channel.Id).OnMessageBroadcast(new MessageBroadcast(message));
    }
}
