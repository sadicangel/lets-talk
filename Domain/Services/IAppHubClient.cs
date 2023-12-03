using LetsTalk.Events;

namespace LetsTalk.Services;
public interface IAppHubClient
{
    Task OnUserConnected(UserConnected @event);
    Task OnUserDisconnected(UserDisconnected @event);

    Task OnChannelCreated(ChannelCreated @event);
    Task OnChannelDeleted(ChannelDeleted @event);

    Task OnUserJoined(UserJoined @event);
    Task OnUserLeft(UserLeft @event);

    Task OnServerMessageBroadcast(ServerMessageBroadcast @event);
    Task OnChannelMessageBroadcast(ChannelMessageBroadcast @event);
}