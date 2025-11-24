using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Hub.ConnectUser;

public sealed record class ConnectUserCommand(UserIdentity User, string ConnectionId) : IRequest;

public sealed class ConnectUserCommandHandler(ConnectionManager connectionManager, ChatDbContext dbContext, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<ConnectUserCommandHandler> logger) : IRequestHandler<ConnectUserCommand>
{
    public async ValueTask<Unit> Handle(ConnectUserCommand request, CancellationToken cancellationToken)
    {
        // User connected
        connectionManager.AddConnection(request.User, request.ConnectionId);
        logger.LogInformation("User connected: {@User} ({ConnectionId})", request.User, request.ConnectionId);

        // Add user to groups based on channels
        var userChannels = await dbContext.Members.AsNoTracking()
            .Where(m => m.UserId == request.User.UserId)
            .ToArrayAsync(cancellationToken: cancellationToken);

        await Task.WhenAll(userChannels.Select(channel => hubContext.Groups.AddToGroupAsync(request.ConnectionId, channel.ChannelId, cancellationToken)));

        // Notify users in the same groups about user joining (for the first time)
        var connections = connectionManager.GetConnections(request.User.UserId);
        if (connections.Count != 1)
        {
            return Unit.Value;
        }

        await hubContext.Clients.All.OnUserConnected(new UserConnected(request.User, connectionManager.GetOnlineUsers()));
        logger.LogInformation("User has joined the server: {@User} ({ConnectionId})", request.User, request.ConnectionId);

        return Unit.Value;
    }
}
