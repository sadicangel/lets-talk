using LetsTalk.Events;

namespace LetsTalk.Services;
public interface IAppHubClient
{
    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    Task OnChannelCreated(ChannelCreated @event);
    Task OnChannelUpdated(ChannelUpdated @event);
    Task OnChannelDeleted(ChannelDeleted @event);

    Task OnUserJoined(UserJoined @event);
    Task OnUserLeft(UserLeft @event);

    Task OnMessageBroadcast(MessageBroadcast @event);
    Task OnNotificationBroadcast(NotificationBroadcast @event);
}