using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace LetsTalk.ChatClient.Console.Services;
public sealed class LoggingLetsTalkClient(HubConnection connection, ILogger<LoggingLetsTalkClient> logger) : LetsTalkClient(connection)
{
    public override Task OnChannelMemberJoined(ChannelMemberJoined @event)
    {
        logger.LogInformation("[User Joined] {@UserName} joined {@Channel}", @event.JoiningUser.UserName, @event.Channel.ChannelName);

        return Task.CompletedTask;
    }

    public override Task OnChannelMemberLeft(ChannelMemberLeft @event)
    {
        logger.LogInformation("[User Left] {@UserName} left {@Channel}", @event.LeavingUser.UserName, @event.Channel.ChannelName);

        return Task.CompletedTask;
    }

    public override Task OnMessage(ChannelMessage @event)
    {
        logger.LogInformation("[Message] (from {@UserName}): {@Message}", @event.Author.UserName, System.Text.Encoding.UTF8.GetString(@event.Content));

        return Task.CompletedTask;
    }

    public override Task OnNotification(ServerNotification @event)
    {
        logger.LogInformation("[Notification]: {@Message}", System.Text.Encoding.UTF8.GetString(@event.Content));

        return Task.CompletedTask;
    }

    public override Task OnUserConnected(UserConnected @event)
    {
        logger.LogInformation("[User Connected] {@UserName} (Total online: {@OnlineUsers})", @event.ConnectingUser.UserName, @event.OnlineUsers.Count());

        return Task.CompletedTask;
    }

    public override Task OnUserDisconnected(UserDisconnected @event)
    {
        logger.LogInformation("[User Disconnected] {@UserName} (Total online: {@OnlineUsers})", @event.DisconnectingUser.UserName, @event.OnlineUsers.Count());

        return Task.CompletedTask;
    }
}
