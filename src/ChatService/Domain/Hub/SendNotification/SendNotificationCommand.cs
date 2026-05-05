using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Hub.SendNotification;

public sealed record class SendNotificationCommand(string ContentType, byte[] Content) : IRequest;

public sealed class SendNotificationCommandHandler(IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<SendNotificationCommandHandler> logger) : IRequestHandler<SendNotificationCommand>
{
    public async ValueTask<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification(request.ContentType, request.Content);

        // Send notification to all clients in the group.
        await hubContext.Clients.All.OnNotification(notification);

        logger.LogInformation("Sent notification: {@Notification}", notification);

        // await publishEndpoint.Publish(notification);

        return Unit.Value;
    }
}
