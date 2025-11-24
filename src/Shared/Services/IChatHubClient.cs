using LetsTalk.Events;

namespace LetsTalk.Services;

public interface IChatHubClient
{
    Task OnMessage(Message @event);
    Task OnNotification(Notification @event);

    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    Task OnChannelMemberJoined(ChannelMemberJoined @event);
    Task OnChannelMemberLeft(ChannelMemberLeft @event);
}
