﻿using LetsTalk.Domain.Events;

namespace LetsTalk.Domain.Services;

public interface ILetsTalkClient
{
    Task OnMessage(MessageEvent @event);
    Task OnNotification(NotificationEvent @event);

    Task OnUserConnected(UserConnectedEvent @event);
    Task OnUserDisconnected(UserDisconnectedEvent @event);

    Task OnChannelMemberJoined(ChannelMemberJoinedEvent @event);
    Task OnChannelMemberLeft(ChannelMemberLeftEvent @event);
}
