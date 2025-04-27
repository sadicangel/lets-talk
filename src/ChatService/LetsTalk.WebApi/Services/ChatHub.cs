using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi.Services;

internal sealed class ChatHub(ILogger<ChatHub> logger)
    : Hub<ILetsTalkClient>
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("User connected: {UserName}#{UserId} (Connection: {Connection}",
            Context.User?.Identity?.Name, Context.UserIdentifier, Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("User disconnected: {UserName}#{UserId} (Connection: {Connection}",
            Context.User?.Identity?.Name, Context.UserIdentifier, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}
