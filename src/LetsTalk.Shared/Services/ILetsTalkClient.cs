using LetsTalk.Shared.Events;

namespace LetsTalk.Shared.Services;

public interface ILetsTalkClient
{
    Task OnMessage(ChannelMessage @event);
    Task OnNotification(ServerNotification @event);

    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    Task OnChannelMemberJoined(ChannelMemberJoined @event);
    Task OnChannelMemberLeft(ChannelMemberLeft @event);
}
