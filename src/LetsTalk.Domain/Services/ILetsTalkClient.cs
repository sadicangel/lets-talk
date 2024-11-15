using LetsTalk.Domain.Events;

namespace LetsTalk.Domain.Services;

public interface ILetsTalkClient
{
    Task OnMessageEvent(MessageEvent @event);
    Task OnNotificationEvent(NotificationEvent @event);
}
