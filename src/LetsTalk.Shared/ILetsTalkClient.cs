using LetsTalk.Shared.Events;

namespace LetsTalk.Shared;

public interface ILetsTalkClient
{
    Task OnMessage(ChannelMessage @event);
    //Task OnNotification(NotificationEvent @event);

    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    Task OnChannelMemberJoined(ChannelMemberJoined @event);
    Task OnChannelMemberLeft(ChannelMemberLeft @event);
}
