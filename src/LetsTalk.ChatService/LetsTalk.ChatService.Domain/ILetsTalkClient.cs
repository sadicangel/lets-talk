using LetsTalk.ChatService.Domain.Events;

namespace LetsTalk.ChatService.Domain;

public interface ILetsTalkClient
{
    //Task OnMessage(MessageEvent @event);
    //Task OnNotification(NotificationEvent @event);

    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    //Task OnChannelMemberJoined(ChannelMemberJoinedEvent @event);
    //Task OnChannelMemberLeft(ChannelMemberLeftEvent @event);
}
