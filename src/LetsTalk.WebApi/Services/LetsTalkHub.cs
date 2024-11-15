using System.Security.Claims;
using LetsTalk.Domain.Events;
using LetsTalk.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi.Services;

[Authorize]
internal sealed class LetsTalkHub : Hub<ILetsTalkClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.OnNotificationEvent(new NotificationEvent("A new user has joined the chat room."));

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.OnNotificationEvent(new NotificationEvent("A user has left the chat room."));

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid channelId, string contentType, byte[] content)
    {
        var senderId = Guid.Parse(Context.GetHttpContext()!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await Clients.All.OnMessageEvent(new MessageEvent(channelId, senderId, contentType, content));
    }
}
