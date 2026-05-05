using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Hub.DisconnectUser;

public sealed record class DisconnectUserCommand(UserIdentity User, string ConnectionId) : IRequest;

public sealed class DisconnectUserCommandHandler(ConnectionManager connectionManager, ChatDbContext dbContext, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<DisconnectUserCommandHandler> logger) : IRequestHandler<DisconnectUserCommand>
{
    public async ValueTask<Unit> Handle(DisconnectUserCommand request, CancellationToken cancellationToken)
    {
        // User disconnected
        connectionManager.RemoveConnection(request.ConnectionId);
        logger.LogInformation("User disconnected: {@User} ({ConnectionId})", request.User, request.ConnectionId);

        // Remove user from groups
        var userChannels = await dbContext.Members.AsNoTracking()
            .Where(m => m.UserId == request.User.UserId)
            .ToArrayAsync(cancellationToken: cancellationToken);
        await Task.WhenAll(userChannels.Select(channel => hubContext.Groups.RemoveFromGroupAsync(request.ConnectionId, channel.ChannelId, cancellationToken)));

        // Notify users in the same groups about user leaving
        var connections = connectionManager.GetConnections(request.User.UserId);
        if (connections.Count != 0)
        {
            return Unit.Value;
        }

        await hubContext.Clients.All.OnUserDisconnected(new UserDisconnected(request.User, connectionManager.GetOnlineUsers()));
        logger.LogInformation("User has left the server: {@User} ({ConnectionId})", request.User, request.ConnectionId);

        return Unit.Value;
    }
}
