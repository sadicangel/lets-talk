using LetsTalk.Chat.Entities;
using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Hub.SendMessage;

public sealed record class SendMessageCommand(UserIdentity User, string ChannelId, string ContentType, byte[] Content) : IRequest;

public sealed class SendMessageCommandHandler(ChatDbContext dbContext, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<SendMessageCommandHandler> logger) : IRequestHandler<SendMessageCommand>
{
    public async ValueTask<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.FindAsync<Channel>([request.ChannelId], cancellationToken) ??
            throw new HubException($"Channel with ID '{request.ChannelId}' does not exist.");

        var message = new Message(channel.ToIdentity(), request.User, request.ContentType, request.Content);

        // Send message to all clients in the group.
        await hubContext.Clients.Group(request.ChannelId).OnMessage(message);

        logger.LogInformation("User {UserId} sent message to channel {ChannelId}: {@Message}", request.User, request.ChannelId, message);

        // await publishEndpoint.Publish(message);

        return Unit.Value;
    }
}
